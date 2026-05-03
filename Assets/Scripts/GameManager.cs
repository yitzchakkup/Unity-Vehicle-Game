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
    
    public int GetStarsCollected() => starsCollected;
    public int GetTotalStars() => totalStars;
    public int GetObstaclesHit() => obstaclesHit;
}
