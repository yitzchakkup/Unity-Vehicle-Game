## Plan: Unity Vehicle Controller System

This plan outlines implementing a physics-based vehicle controller in Unity using the existing Input System setup, transforming the current transform-based movement to Rigidbody-driven physics for realistic handling, and refining the camera follow script for smooth tracking. It builds on the current PlayerController.cs (transform movement), CameraScript.cs (basic follow with duplicate code), and InputSystem_Actions.inputactions (Move action configured).

### Steps
1. Attach Rigidbody component to the player GameObject in the scene for physics simulation.
2. Update PlayerController.cs to use Rigidbody for movement: replace transform.Translate/Rotate with Rigidbody.AddForce for acceleration and AddTorque for steering.
3. Fix CameraScript.cs by removing duplicate class definitions and ensuring the script uses TransformPoint and LookAt for rotation-aware following.
4. Assign the Move InputActionReference in PlayerController's Inspector to link with InputSystem_Actions.
5. Test vehicle movement in Play mode: verify forward/backward acceleration, turning, and camera following without clipping or jitter.

### Further Considerations
1. Adjust Rigidbody settings (mass, drag, constraints) for vehicle feel; consider adding WheelColliders for advanced physics.
2. Ensure Input System package is installed and active; add error handling for missing Rigidbody or InputAction.
3. Test on different terrains; add ground detection to prevent flying if needed.
