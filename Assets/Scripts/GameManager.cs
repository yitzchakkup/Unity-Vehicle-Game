using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Counters")]
    private int _starsCollected = 0;
    private int _totalStars = 0;
    private int _obstaclesHit = 0;
    
    [Header("UI References")]
    [Tooltip("Drag the Stars Text here")]
    [SerializeField] private TextMeshProUGUI starScoreDisplay;
    [Tooltip("Drag the Obstacles Text here")]
    [SerializeField] private TextMeshProUGUI obstacleHitDisplay;
    [Tooltip("Drag a large center screen TextMeshPro here for warnings/messages")]
    [SerializeField] private TextMeshProUGUI warningMessageDisplay;
    
    [Header("Game Over UI")]
    [Tooltip("Drag the GameObject containing your Game Over screen (Text and Buttons) here")]
    [SerializeField] private GameObject gameOverPanel;
    [Tooltip("Drag the TextMeshPro on the Game Over screen that will show the final message")]
    [SerializeField] private TextMeshProUGUI gameOverMessageText;
    
    [Header("Prefabs")]
    [Tooltip("Drag the Star prefab here so we can respawn it if destroyed")]
    [SerializeField] private GameObject starPrefab;
    
    private bool _isGameOver = false;

    // We store the original stars when the game first starts, so we can respawn them
    private Star[] _initialStars;
    private Vector3[] _initialStarPositions;

    private void Awake()
    {
        // Singleton pattern
        Instance = this;
    }
    
    private void Start()
    {
        // Save the exact state of all stars in the scene at the very beginning
        _initialStars = FindObjectsByType<Star>(FindObjectsSortMode.None);
        _totalStars = _initialStars.Length;
        
        _initialStarPositions = new Vector3[_initialStars.Length];
        for (int i = 0; i < _initialStars.Length; i++)
        {
            _initialStarPositions[i] = _initialStars[i].transform.position;
        }

        ResetGameData();
    }

    private void ResetGameData()
    {
        _starsCollected = 0;
        _obstaclesHit = 0;
        _isGameOver = false;

        // Ensure regular UI is visible
        if (starScoreDisplay != null) starScoreDisplay.gameObject.SetActive(true);
        if (obstacleHitDisplay != null) obstacleHitDisplay.gameObject.SetActive(true);
        if (warningMessageDisplay != null)
        {
            warningMessageDisplay.gameObject.SetActive(true);
            warningMessageDisplay.text = "";
        }

        // Hide Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateDisplay();
    }
    
    public void AddStarScore()
    {
        if (_isGameOver) return;
        
        _starsCollected++;
        UpdateDisplay();
        
        Debug.Log($"Star collected! Progress: {_starsCollected}/{_totalStars}");
        
        CheckWinCondition();
    }
    
    public void AddObstacleHit()
    {
        if (_isGameOver) return;
        
        _obstaclesHit++;
        UpdateDisplay();
        
        Debug.Log($"Ouch! You have hit {_obstaclesHit} obstacles so far!");
    }
    
    private void CheckWinCondition()
    {
        // If all stars are collected, trigger the Game Over screen
        if (_starsCollected >= _totalStars && _totalStars > 0)
        {
            ShowGameOverScreen();
        }
    }
    
    private void ShowGameOverScreen()
    {
        _isGameOver = true;
        
        // Stop the player's car
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.StopCar();
        }

        // Hide the in-game UI to make the screen look clean
        if (starScoreDisplay != null) starScoreDisplay.gameObject.SetActive(false);
        if (obstacleHitDisplay != null) obstacleHitDisplay.gameObject.SetActive(false);
        if (warningMessageDisplay != null) warningMessageDisplay.gameObject.SetActive(false);

        if (gameOverPanel != null && gameOverMessageText != null)
        {
            // Activate the panel
            gameOverPanel.SetActive(true);
            
            // Determine the message and color based on performance
            if (_obstaclesHit >= 3)
            {
                // Orange color (RGB: 1, 0.5, 0)
                gameOverMessageText.color = new Color(1f, 0.5f, 0f, 1f);
                gameOverMessageText.text = "You could do better, try racing the car less.\nTry again?";
            }
            else
            {
                // Green color
                gameOverMessageText.color = Color.green;
                gameOverMessageText.text = "Congratulations!\nWould you like another game?";
            }
        }
        else
        {
            Debug.LogError("GameManager: Game Over Panel or Message Text is not assigned in the Inspector!");
        }
    }
    
    // This public method should be linked to the "Play Again" button's OnClick event in Unity
    public void RestartGame()
    {
        Debug.Log("Restarting game without reloading the scene...");
        
        // 1. Reset variables and UI
        ResetGameData();

        // 2. Send the car back to the starting line
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ResetToStart();
        }
        
        // 3. Bring all the original stars back to life!
        for (int i = 0; i < _initialStars.Length; i++)
        {
            if (_initialStars[i] == null)
            {
                // If the star was somehow destroyed, we need to spawn a new one from the prefab
                if (starPrefab != null)
                {
                    GameObject newStarObj = Instantiate(starPrefab, _initialStarPositions[i], Quaternion.identity);
                    _initialStars[i] = newStarObj.GetComponent<Star>();
                }
                else
                {
                    Debug.LogError("GameManager: Cannot respawn star because Star Prefab is missing in the Inspector!");
                }
            }
            else
            {
                // The star wasn't destroyed, just hidden. Tell it to reset itself.
                _initialStars[i].ResetStar();
            }
        }
    }

    private void UpdateDisplay()
    {
        if (starScoreDisplay != null)
        {
            starScoreDisplay.text = $"cur points: {_starsCollected} / {_totalStars}";
        }
        
        if (obstacleHitDisplay != null)
        {
            obstacleHitDisplay.text = $"Obstacle Hits: {_obstaclesHit}";
        }
    }
    
    /// <summary>
    /// Displays a message on the screen for a specific duration.
    /// </summary>
    public void ShowMessage(string message, float duration)
    {
        if (warningMessageDisplay != null && warningMessageDisplay.gameObject.activeInHierarchy)
        {
            StopAllCoroutines(); // Stop any existing message timer
            StartCoroutine(DisplayMessageRoutine(message, duration));
        }
    }

    private IEnumerator DisplayMessageRoutine(string message, float duration)
    {
        warningMessageDisplay.text = message;
        yield return new WaitForSeconds(duration);
        warningMessageDisplay.text = "";
    }
    
    public int GetStarsCollected() => _starsCollected;
    public int GetTotalStars() => _totalStars;
    public int GetObstaclesHit() => _obstaclesHit;
}
