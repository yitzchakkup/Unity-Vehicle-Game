using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 40.0f;
    [SerializeField] private float turnSpeed = 140.0f; // Dedicated turn speed
    
    // Move action will be set by GameManager
    private InputActionReference moveAction;
    
    // Public method for GameManager to set the move action
    public void SetMoveAction(InputActionReference action)
    {
        moveAction = action;
        if (moveAction != null)
            moveAction.action.Enable();
    }
    
    void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    void Update()
    {
        if (moveAction is null) return;

        // Get movement input from the Input System
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        
        // Move forward/backward based on vertical input
        transform.Translate(Vector3.forward * (moveInput.y * moveSpeed * Time.deltaTime));
        
        // Turn left/right based on horizontal input
        transform.Rotate(Vector3.up, moveInput.x * turnSpeed * Time.deltaTime);
    }
}