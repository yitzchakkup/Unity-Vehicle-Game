using UnityEngine;

namespace Prefabs
{
    public class VehicleManager : MonoBehaviour
    {
        public static VehicleManager Instance { get; private set; }//setting this variable is private
        
        [Header("Prefabs")]
        [SerializeField] private GameObject vehiclePrefab;

        
        // We removed the Input Action from here, since PlayerController now manages it directly.
        
        [Header("Spawned Objects")]
        [SerializeField] private GameObject currentVehicle;
        
        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Find the vehicle and environment in the scene if they exist
            if (currentVehicle == null)
            {
                PlayerController controller = FindFirstObjectByType<PlayerController>();
                if (controller != null)
                {
                    currentVehicle = controller.gameObject;
                }
            }
            
        }
        
        public GameObject GetCurrentVehicle()
        {
            return currentVehicle;
        }
        
        public void SetCurrentVehicle(GameObject vehicle)
        {
            currentVehicle = vehicle;
        }
        
        
        public GameObject GetVehiclePrefab()
        {
            return vehiclePrefab;
        }

    }
}
