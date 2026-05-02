using UnityEngine;
using TMPro;

public class StarCollector : MonoBehaviour
{
    private int _starsCollected = 0;
    private int _totalStars = 0;
    
    [SerializeField] private TextMeshProUGUI scoreDisplay; // For UI display (optional)
    
    private static StarCollector _instance;
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Count total stars in the scene
        Star[] starsInScene = FindObjectsOfType<Star>();
        _totalStars = starsInScene.Length;
        
        UpdateDisplay();
    }
    
    public void AddScore()
    {
        _starsCollected++;
        UpdateDisplay();
        
        Debug.Log($"Star collected! Progress: {_starsCollected}/{_totalStars}");
        
        // Optional: Play a sound or effect here
    }
    
    private void UpdateDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = $"Stars: {_starsCollected}/{_totalStars}";
        }
    }
    
    public int GetStarsCollected()
    {
        return _starsCollected;
    }
    
    public int GetTotalStars()
    {
        return _totalStars;
    }
}

