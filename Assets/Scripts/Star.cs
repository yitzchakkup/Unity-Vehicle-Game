using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Rotation speed for visual effect
    [SerializeField] private float bobHeight = 0.5f; // How much it bobs up and down
    [SerializeField] private float bobSpeed = 2f; // Speed of bobbing motion
    
    [Header("Audio")]
    [Tooltip("The sound to play when the star is collected")]
    [SerializeField] private AudioClip collectSound;
    [Tooltip("Volume of the collection sound (0.0 to 1.0)")]
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1.0f;
    
    private Vector3 _startPosition;
    private bool _isCollected = false;
    
    private void Start()
    {
        _startPosition = transform.position;
        
        // Add a trigger collider if not present
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 1.5f; // Ensure it's big enough to hit
        }
        else
        {
            col.isTrigger = true;
        }
    }
    
    private void Update()
    {
        if (_isCollected) return;
        
        // Rotate the star
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        
        // Bob up and down
        Vector3 newPos = _startPosition;
        newPos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = newPos;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the vehicle hit the star
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            CollectStar();
        }
    }
    
    private void CollectStar()
    {
        if (_isCollected) return;
        
        _isCollected = true;
        
        if (collectSound != null)
        {
            Vector3 soundPosition = transform.position;
            
            // Try to find the main camera so the sound is always at full volume to the player
            if (Camera.main != null)
            {
                soundPosition = Camera.main.transform.position;
            }
            
            AudioSource.PlayClipAtPoint(collectSound, soundPosition, soundVolume);
        }
        else
        {
            Debug.LogWarning("Star collected, but no Collect Sound was assigned in the Inspector!");
        }
        
        // Notify the new GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddStarScore();
        }
        
        // Hide the star instead of destroying it
        gameObject.SetActive(false);
    }
    
    public void ResetStar()
    {
        _isCollected = false;
        gameObject.SetActive(true);
    }
}
