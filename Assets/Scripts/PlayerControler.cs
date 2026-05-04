using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
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

    // Track if the player is currently allowed to move by external scripts (like a traffic light barrier)
    private bool _canReceiveInput = true;
    private Rigidbody _rb;
    private Vector2 _moveInput;

    public void SetMoveAction(InputActionReference action)
    {
        moveAction = action;
        if (moveAction != null && _canReceiveInput)
            moveAction.action.Enable();
    }

    void OnEnable()
    {
        // Automatically enable the action when the script starts
        if (moveAction != null && _canReceiveInput)
        {
            moveAction.action.Enable();
        }
    }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        // Record the starting position and rotation
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        
        if (_rb != null)
        {
            // We lock X and Z rotation to stop flipping, but keep Y free for steering.
            // We let gravity handle the Y position naturally.
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // PREVENT TUNNELING: Force the physics engine to calculate collisions for this object 
            // continuously rather than in discrete chunks. This stops fast cars from teleporting through walls!
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
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
        // Gather input during Update, but apply physics in FixedUpdate
        if (moveAction == null || !_canReceiveInput)
        {
            _moveInput = Vector2.zero;
            return;
        }
        
        _moveInput = moveAction.action.ReadValue<Vector2>();
    }

    // FixedUpdate is called in sync with the physics engine.
    // It's the only correct place to apply physics forces or modify Rigidbody velocities.
    void FixedUpdate()
    {
        if (_rb == null) return;
        
        // Move the car forward/backward based on vertical input (Gas/Brake)
        // We calculate the new physical position and ask the physics engine to push the car there.
        // This completely prevents glitching through walls or jittering against barriers.
        Vector3 movement = transform.forward * (_moveInput.y * moveSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(_rb.position + movement);

        // Turn the car based on horizontal input (Steering)
        // We use a physically accurate torque instead of teleporting rotation
        Quaternion turnRotation = Quaternion.Euler(0f, _moveInput.x * turnSpeed * Time.fixedDeltaTime, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);

        // Keep the player within the road boundaries
        if (useBoundaries)
        {
            ClampPosition();
        }
    }

    // When the car physically hits a wall/barrier
    private void OnCollisionEnter(Collision collision)
    {
        // General collision logic to stop bouncing when hitting specific objects
        if (collision.gameObject.name == "pCube1" || collision.gameObject.GetComponent<TrafficLightBarrier>() != null)
        {
            if (_rb != null)
            {
                // Kill momentum instantly to prevent bouncing
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Stops the car's movement and disables input.
    /// </summary>
    public void StopCar()
    {
        _canReceiveInput = false;
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        Debug.Log("Car stopped by external command.");
    }

    /// <summary>
    /// Allows the car to move and enables input.
    /// </summary>
    public void StartCar()
    {
        _canReceiveInput = true;
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
        Debug.Log("Car allowed to move by external command.");
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
        
        // Ensure the car is allowed to move again after respawning
        StartCar();
        
        // If the car has a Rigidbody, we must reset its velocity so it doesn't keep flying
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Car reset to starting position!");
    }

    private void ClampPosition()
    {
        Vector2 currentPos2D = new Vector2(_rb.position.x, _rb.position.z);
        
        // If the current position is outside the polygon
        if (!IsPointInPolygon(currentPos2D, _roadPolygon))
        {
            // Simple collision resolution: push the car back to the closest point on the boundary
            Vector2 closestPoint = GetClosestPointOnPolygon(currentPos2D, _roadPolygon);
            
            // Apply the corrected position via physics, not teleportation
            _rb.MovePosition(new Vector3(closestPoint.x, _rb.position.y, closestPoint.y));
            
            // Optional: Kill momentum when hitting the invisible side barriers so it doesn't scrape wildly
            _rb.linearVelocity = Vector3.zero;
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
