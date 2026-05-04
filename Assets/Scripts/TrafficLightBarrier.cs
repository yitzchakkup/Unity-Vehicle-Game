using UnityEngine;

public class TrafficLightBarrier : MonoBehaviour
{
    [Tooltip("Drag the TrafficLight script associated with this barrier here.")]
    [SerializeField] private TrafficLight associatedTrafficLight;
    
    [Tooltip("The sound to play when running a red/yellow light (optional)")]
    [SerializeField] private AudioClip penaltySound;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1.0f;

    private Collider _barrierCollider;

    private void Start()
    {
        // Get the collider attached to this barrier
        _barrierCollider = GetComponent<Collider>();
        if (_barrierCollider == null)
        {
            Debug.LogError("TrafficLightBarrier: This object needs a Collider (e.g., BoxCollider)!");
        }
        else
        {
            // The user wants this to be a trigger now, so they pass through it and get reset
            _barrierCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (associatedTrafficLight == null) return;
        
        // Check if the car hit the trigger
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // If the light is NOT green, penalize them!
            if (associatedTrafficLight.GetCurrentState() != TrafficLight.LightState.Green)
            {
                Debug.Log("Player ran a red/yellow light! Resetting to start.");
                
                // Show a message on the screen via GameManager
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.ShowMessage("Do NOT cross on Yellow or Red!", 2f);
                }
                
                // Reset the player
                player.ResetToStart(penaltySound, soundVolume);
            }
        }
    }
}
