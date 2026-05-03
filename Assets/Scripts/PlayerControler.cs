using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 40.0f;
    [SerializeField] private float turnSpeed = 140.0f;
    
    [Header("Road Boundaries")]
    [Tooltip("Enable to manually lock the vehicle within the bounds of the road.")]
    [SerializeField] private bool useBoundaries = true;
    
    // We will store the polygon that represents the road.
    private readonly Vector2[] _roadPolygon = new Vector2[]
    {
        new Vector2(-22.13652f, -15.15427f),
        new Vector2(-37.2852f, -13.05346f),
        new Vector2(-38.0466f, 347.4919f),
        new Vector2(-22.05066f, 342.8215f)
    };
    
    [Header("Input Action")]
    [Tooltip("Assign the Move action here directly in the Inspector")]
    [SerializeField] private InputActionReference moveAction;
    
    // Variables to store the starting state
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    
    public void SetMoveAction(InputActionReference action)
    {
        moveAction = action;
        if (moveAction != null)
            moveAction.action.Enable();
    }

    void OnEnable()
    {
        // Automatically enable the action when the script starts
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }
    
    void Start()
    {
        // Record the starting position and rotation
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    void Update()
    {
        if (moveAction == null) return;

        // Get movement input from the Input System
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        
        // Store previous position before moving
        Vector3 previousPosition = transform.position;
        
        // Move forward/backward based on vertical input
        transform.Translate(Vector3.forward * (moveInput.y * moveSpeed * Time.deltaTime));
        
        // Turn left/right based on horizontal input
        transform.Rotate(Vector3.up, moveInput.x * turnSpeed * Time.deltaTime);

        // Keep the player within the road boundaries
        if (useBoundaries)
        {
            ClampPosition(previousPosition);
        }
    }
    
    // Public method to reset the car to its starting position, optionally playing a sound
    public void ResetToStart(AudioClip crashSound = null, float soundVolume = 1.0f)
    {
        // Play the provided crash sound exactly where the car is before it teleports back
        if (crashSound != null)
        {
            Vector3 soundPosition = transform.position;
            if (Camera.main != null)
            {
                soundPosition = Camera.main.transform.position;
            }
            
            AudioSource.PlayClipAtPoint(crashSound, soundPosition, soundVolume);
        }

        // Reset position and rotation
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        
        // If the car has a Rigidbody, we must reset its velocity so it doesn't keep flying
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Car reset to starting position!");
    }

    private void ClampPosition(Vector3 previousPosition)
    {
        Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.z);
        
        // If the current position is outside the polygon
        if (!IsPointInPolygon(currentPos2D, _roadPolygon))
        {
            // Simple collision resolution: push the car back to the closest point on the boundary
            Vector2 closestPoint = GetClosestPointOnPolygon(currentPos2D, _roadPolygon);
            
            // Apply the corrected position (keeping the original Y height)
            transform.position = new Vector3(closestPoint.x, transform.position.y, closestPoint.y);
        }
    }
    
    // Ray-casting algorithm to determine if a point is inside a polygon
    private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool isInside = false;
        int j = polygon.Length - 1;
        
        for (int i = 0; i < polygon.Length; i++)
        {
            if (polygon[i].y < point.y && polygon[j].y >= point.y || polygon[j].y < point.y && polygon[i].y >= point.y)
            {
                if (polygon[i].x + (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < point.x)
                {
                    isInside = !isInside;
                }
            }
            j = i;
        }
        
        return isInside;
    }
    
    // Finds the absolute closest point on the edges of the polygon to snap the car back
    private Vector2 GetClosestPointOnPolygon(Vector2 point, Vector2[] polygon)
    {
        Vector2 closestPoint = point;
        float minDistance = float.MaxValue;
        
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[(i + 1) % polygon.Length];
            
            Vector2 closestOnSegment = GetClosestPointOnLineSegment(point, p1, p2);
            float dist = Vector2.Distance(point, closestOnSegment);
            
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPoint = closestOnSegment;
            }
        }
        
        return closestPoint;
    }
    
    private Vector2 GetClosestPointOnLineSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / Vector2.Dot(ab, ab);
        
        if (t < 0.0f) return a;
        if (t > 1.0f) return b;
        
        return a + t * ab;
    }
}
