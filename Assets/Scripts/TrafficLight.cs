using System.Collections;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum LightState { Green, Yellow, Red }
    
    [Header("Timings (in seconds)")]
    [SerializeField] private float greenDuration = 5f;
    [SerializeField] private float yellowDuration = 2f;
    [SerializeField] private float redDuration = 5f;

    [Header("Light Renderers")]
    [Tooltip("Drag pSphere9 here")]
    [SerializeField] private Renderer greenLightRenderer;
    [Tooltip("Drag pSphere8 here")]
    [SerializeField] private Renderer yellowLightRenderer;
    [Tooltip("Drag pSphere7 here")]
    [SerializeField] private Renderer redLightRenderer;
    
    [Header("Colors & Emission")]
    [ColorUsage(false, true)] // Enables HDR color picker for glowing effect
    [SerializeField] private Color greenGlow = new Color(0f, 1f, 0f, 1f) * 2f;
    [ColorUsage(false, true)]
    [SerializeField] private Color yellowGlow = new Color(1f, 0.92f, 0.016f, 1f) * 2f;
    [ColorUsage(false, true)]
    [SerializeField] private Color redGlow = new Color(1f, 0f, 0f, 1f) * 2f;
    
    [Tooltip("The color when a light is off")]
    [SerializeField] private Color offColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    private LightState currentState = LightState.Green;
    
    // We store instanced materials so we don't accidentally modify the global asset
    private Material greenMat;
    private Material yellowMat;
    private Material redMat;

    private void Start()
    {
        // Initialize materials if the renderers are assigned
        if (greenLightRenderer != null)
        {
            greenMat = greenLightRenderer.material;
            greenMat.EnableKeyword("_EMISSION");
        }
        else Debug.LogError("Green Light Renderer (pSphere9) is not assigned!");

        if (yellowLightRenderer != null)
        {
            yellowMat = yellowLightRenderer.material;
            yellowMat.EnableKeyword("_EMISSION");
        }
        else Debug.LogError("Yellow Light Renderer (pSphere8) is not assigned!");

        if (redLightRenderer != null)
        {
            redMat = redLightRenderer.material;
            redMat.EnableKeyword("_EMISSION");
        }
        else Debug.LogError("Red Light Renderer (pSphere7) is not assigned!");

        // Start the cycle if everything is assigned
        if (greenMat != null && yellowMat != null && redMat != null)
        {
            StartCoroutine(TrafficLightCycle());
        }
    }

    private IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            switch (currentState)
            {
                case LightState.Green:
                    UpdateLights(greenGlow, offColor, offColor);
                    yield return new WaitForSeconds(greenDuration);
                    currentState = LightState.Yellow;
                    break;
                    
                case LightState.Yellow:
                    UpdateLights(offColor, yellowGlow, offColor);
                    yield return new WaitForSeconds(yellowDuration);
                    currentState = LightState.Red;
                    break;
                    
                case LightState.Red:
                    UpdateLights(offColor, offColor, redGlow);
                    yield return new WaitForSeconds(redDuration);
                    currentState = LightState.Green;
                    break;
            }
        }
    }

    private void UpdateLights(Color greenEmission, Color yellowEmission, Color redEmission)
    {
        if (greenMat != null) greenMat.SetColor("_EmissionColor", greenEmission);
        if (yellowMat != null) yellowMat.SetColor("_EmissionColor", yellowEmission);
        if (redMat != null) redMat.SetColor("_EmissionColor", redEmission);
    }
    
    /// <summary>
    /// Returns the current state of the traffic light.
    /// Can be Green, Yellow, or Red.
    /// </summary>
    public LightState GetCurrentState()
    {
        return currentState;
    }
}
