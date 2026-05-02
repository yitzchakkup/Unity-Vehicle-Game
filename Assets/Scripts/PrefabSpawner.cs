using Prefabs;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public void SpawnVehicle(GameObject vehiclePrefab, Vector3 position = default)
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
        
        // Spawn new vehicle
        GameObject newVehicle = Instantiate(vehiclePrefab, position, Quaternion.identity);
        VehicleManager.Instance.SetCurrentVehicle(newVehicle);
        
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
        Vector3 currentPosition = Vector3.zero;
        
        // Get current vehicle position if it exists
        if (VehicleManager.Instance.GetCurrentVehicle() != null)
        {
            currentPosition = VehicleManager.Instance.GetCurrentVehicle().transform.position;
        }
        
        SpawnVehicle(newVehiclePrefab, currentPosition);
    }
    
    public void ChangeEnvironment(GameObject newEnvironmentPrefab)
    {
        SpawnEnvironment(newEnvironmentPrefab);
    }
}
