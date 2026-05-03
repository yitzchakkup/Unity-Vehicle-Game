using Prefabs;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public void SpawnVehicle(GameObject vehiclePrefab, Vector3 localPosition = default, Transform parentTransform = null)
    {
        if (vehiclePrefab is null)
        {
            Debug.LogError("Vehicle prefab is null!");
            return;
        }
        
        // Destroy existing vehicle if any
        if (VehicleManager.Instance.GetCurrentVehicle() != null)
        {
            Destroy(VehicleManager.Instance.GetCurrentVehicle());
        }
        
        // Spawn new vehicle, passing the parent directly to Instantiate
        // Note: we pass false for instantiateInWorldSpace so that the position we set later is treated as local
        GameObject newVehicle = Instantiate(vehiclePrefab, parentTransform);
        
        // Explicitly set the local position so it behaves exactly like typing into the Inspector
        if (parentTransform != null)
        {
            newVehicle.transform.localPosition = localPosition;
            newVehicle.transform.localRotation = Quaternion.identity; // Reset rotation relative to parent
        }
        else
        {
            newVehicle.transform.position = localPosition;
        }

        VehicleManager.Instance.SetCurrentVehicle(newVehicle);
        
        // Ensure input action is assigned to the new vehicle
        PlayerController controller = newVehicle.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetMoveAction(VehicleManager.Instance.GetMoveAction());
        }
        
        Debug.Log($"Spawned vehicle: {newVehicle.name}");
    }
    
    public void SpawnEnvironment(GameObject environmentPrefab, Vector3 position = default)
    {
        if (environmentPrefab == null)
        {
            Debug.LogError("Environment prefab is null!");
            return;
        }
        
        // Destroy existing environment if any
        if (VehicleManager.Instance.GetCurrentEnvironment() != null)
        {
            Destroy(VehicleManager.Instance.GetCurrentEnvironment());
        }
        
        // Spawn new environment
        GameObject newEnvironment = Instantiate(environmentPrefab, position, Quaternion.identity);
        VehicleManager.Instance.SetCurrentEnvironment(newEnvironment);
        
        Debug.Log($"Spawned environment: {newEnvironment.name}");
    }
    
    public void ChangeVehicle(GameObject newVehiclePrefab)
    {
        Vector3 currentLocalPosition = Vector3.zero;
        Transform currentParent = null;
        
        // Get current vehicle local position and parent if it exists
        if (VehicleManager.Instance.GetCurrentVehicle() != null)
        {
            currentParent = VehicleManager.Instance.GetCurrentVehicle().transform.parent;
            currentLocalPosition = currentParent != null 
                ? VehicleManager.Instance.GetCurrentVehicle().transform.localPosition 
                : VehicleManager.Instance.GetCurrentVehicle().transform.position;
        }
        
        SpawnVehicle(newVehiclePrefab, currentLocalPosition, currentParent);
    }
    
    public void ChangeEnvironment(GameObject newEnvironmentPrefab)
    {
        SpawnEnvironment(newEnvironmentPrefab);
    }
}
