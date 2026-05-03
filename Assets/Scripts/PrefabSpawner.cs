using Prefabs;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public void ChangeVehicle(GameObject newVehiclePrefab)
    {
        if (newVehiclePrefab is null)
        {
            Debug.LogError("Vehicle prefab to spawn is null!");
            return;
        }

        GameObject oldVehicle = VehicleManager.Instance.GetCurrentVehicle();
        
        if (oldVehicle == null)
        {
            Debug.LogWarning("No existing vehicle found to replace. Spawning at origin.");
            SpawnVehicle(newVehiclePrefab, Vector3.zero, null, Quaternion.identity);
            return;
        }

        // Save the exact state of the old vehicle
        Transform oldTransform = oldVehicle.transform;
        Vector3 positionToUse = oldTransform.position;
        Quaternion rotationToUse = oldTransform.rotation;
        Transform parentToUse = oldTransform.parent;

        // Destroy the old vehicle
        Destroy(oldVehicle);

        // Spawn the new one in the exact same spot
        SpawnVehicle(newVehiclePrefab, positionToUse, parentToUse, rotationToUse);
    }

    private void SpawnVehicle(GameObject vehiclePrefab, Vector3 worldPosition, Transform parentTransform, Quaternion worldRotation)
    {
        // Instantiate using world position and rotation
        GameObject newVehicle = Instantiate(vehiclePrefab, worldPosition, worldRotation, parentTransform);

        // Update the manager
        VehicleManager.Instance.SetCurrentVehicle(newVehicle);
        
        // Note: The PlayerController now manages its own input action, so we don't need to set it here anymore.
        
        Debug.Log($"Spawned new vehicle: {newVehicle.name} at {worldPosition}");
    }
    
    public void ChangeEnvironment(GameObject newEnvironmentPrefab)
    {
        if (newEnvironmentPrefab == null)
        {
            Debug.LogError("Environment prefab is null!");
            return;
        }
        
        GameObject oldEnvironment = VehicleManager.Instance.GetCurrentEnvironment();
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // Destroy existing environment if any, but save its location
        if (oldEnvironment != null)
        {
            spawnPos = oldEnvironment.transform.position;
            spawnRot = oldEnvironment.transform.rotation;
            Destroy(oldEnvironment);
        }
        
        // Spawn new environment
        GameObject newEnvironment = Instantiate(newEnvironmentPrefab, spawnPos, spawnRot);
        VehicleManager.Instance.SetCurrentEnvironment(newEnvironment);
        
        Debug.Log($"Spawned environment: {newEnvironment.name}");
    }
}
