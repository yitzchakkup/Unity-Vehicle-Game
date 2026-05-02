using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Rotation speed for visual effect
    [SerializeField] private float bobHeight = 0.5f; // How much it bobs up and down
    [SerializeField] private float bobSpeed = 2f; // Speed of bobbing motion
    
    private Vector3 startPosition;
    private bool isCollected = false;
    
    private void Start()
    {
        startPosition = transform.position;
        
        // Add a trigger collider if not present
        if (GetComponent<Collider>() == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
        }
    }
    
    private void Update()
    {
        if (isCollected) return;
        
        // Rotate the star
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        Vector3 newPos = startPosition;
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
        if (isCollected) return;
        
        isCollected = true;
        
        // Notify the score manager
        StarCollector scoreManager = FindObjectOfType<StarCollector>();
        if (scoreManager != null)
        {
            scoreManager.AddScore();
        }
        
        // Destroy this star
        Destroy(gameObject);
    }
}

