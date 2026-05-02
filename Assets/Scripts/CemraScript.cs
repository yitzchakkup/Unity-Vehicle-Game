using Prefabs;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Vector3 _offset = new Vector3(0, 5, -11);
    void Start()
    {

    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        // Get the current vehicle from VehicleManager
        GameObject currentVehicle = VehicleManager.Instance?.GetCurrentVehicle();
        
        if (currentVehicle is not null)
        {
            // Position the camera relative to the vehicle's rotation
            transform.position = currentVehicle.transform.TransformPoint(_offset);
            
            // Make the camera look at the vehicle
            transform.LookAt(currentVehicle.transform);
        }
        else
        {
            Debug.LogWarning("No current vehicle found in VehicleManager!");
        }
    }
}