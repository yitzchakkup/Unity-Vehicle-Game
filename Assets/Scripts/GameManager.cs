using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Counters")]
    private int starsCollected = 0;
    private int totalStars = 0;
    private int obstaclesHit = 0;
    
    [Header("UI References")]
    [Tooltip("Drag the Stars Text here")]
    [SerializeField] private TextMeshProUGUI starScoreDisplay;
    [Tooltip("Drag the Obstacles Text here")]
    [SerializeField] private TextMeshProUGUI obstacleHitDisplay;
    [Tooltip("Drag a large center screen TextMeshPro here for warnings/messages")]
    [SerializeField] private TextMeshProUGUI warningMessageDisplay;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Count total stars in the scene initially
        Star[] starsInScene = FindObjectsByType<Star>(FindObjectsSortMode.None);
        totalStars = starsInScene.Length;
        
        UpdateDisplay();
        
        // Hide the warning message on start
        if (warningMessageDisplay != null)
        {
            warningMessageDisplay.text = "";
        }
    }
    
    public void AddStarScore()
    {
        starsCollected++;
        UpdateDisplay();
        
        Debug.Log($"Star collected! Progress: {starsCollected}/{totalStars}");
    }
    
    public void AddObstacleHit()
    {
        obstaclesHit++;
        UpdateDisplay();
        
        Debug.Log($"Ouch! You have hit {obstaclesHit} obstacles so far!");
    }
    
    private void UpdateDisplay()
    {
        if (starScoreDisplay != null)
        {
            starScoreDisplay.text = $"Stars: {starsCollected} / {totalStars}";
        }
        
        if (obstacleHitDisplay != null)
        {
            obstacleHitDisplay.text = $"Obstacle Hits: {obstaclesHit}";
        }
    }
    
    /// <summary>
    /// Displays a message on the screen for a specific duration.
    /// </summary>
    public void ShowMessage(string message, float duration)
    {
        if (warningMessageDisplay != null)
        {
            StopAllCoroutines(); // Stop any existing message timer
            StartCoroutine(DisplayMessageRoutine(message, duration));
        }
        else
        {
            Debug.LogWarning($"GameManager tried to show message '{message}' but no Warning Message Display UI is assigned!");
        }
    }

    private IEnumerator DisplayMessageRoutine(string message, float duration)
    {
        warningMessageDisplay.text = message;
        yield return new WaitForSeconds(duration);
        warningMessageDisplay.text = "";
    }
    
    public int GetStarsCollected() => starsCollected;
    public int GetTotalStars() => totalStars;
    public int GetObstaclesHit() => obstaclesHit;
}
