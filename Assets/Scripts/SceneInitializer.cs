using Prefabs;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    private void Start()
    {
        InitializeScene();
    }
    
    private void InitializeScene()
    {
        // Get references to the manager components
        VehicleManager vehicleManager = GetComponent<VehicleManager>();
        PrefabSpawner prefabSpawner = GetComponent<PrefabSpawner>();
        
        if (vehicleManager == null)
        {
            Debug.LogError("VehicleManager component not found on this GameObject!");
            return;
        }
        
        if (prefabSpawner == null)
        {
            Debug.LogError("PrefabSpawner component not found on this GameObject!");
            return;
        }
        
        // Spawn initial vehicle and environment
        if (vehicleManager.GetVehiclePrefab() != null)
        {
            prefabSpawner.SpawnVehicle(vehicleManager.GetVehiclePrefab());
        }
        else
        {
            Debug.LogWarning("No vehicle prefab assigned to VehicleManager!");
        }
        
        if (vehicleManager.GetEnvironmentPrefab() != null)
        {
            prefabSpawner.SpawnEnvironment(vehicleManager.GetEnvironmentPrefab());
        }
        else
        {
            Debug.LogWarning("No environment prefab assigned to VehicleManager!");
        }
        
        Debug.Log("Scene initialized successfully!");
    }
}
