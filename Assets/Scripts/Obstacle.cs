using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("The sound to play when this specific obstacle is hit")]
    [SerializeField] private AudioClip hitSound;
    [Tooltip("Volume of the hit sound (0.0 to 1.0)")]
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1.0f;
    
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player (the vehicle) hit the obstacle
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (collision.gameObject.CompareTag("Player") || player != null)
        {
            HitObstacle(player);
        }
    }
    
    private void HitObstacle(PlayerController player)
    {
        // 1. Notify the GameManager that an obstacle was hit
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddObstacleHit();
        }
        
        // 2. Reset the player back to the start, and pass THIS obstacle's sound to the car
        if (player != null)
        {
            player.ResetToStart(hitSound, soundVolume);
        }
    }
}
