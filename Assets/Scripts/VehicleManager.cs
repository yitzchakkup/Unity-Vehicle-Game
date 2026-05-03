using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs
{
    public class VehicleManager : MonoBehaviour
    {
        public static VehicleManager Instance { get; private set; }//setting this variable is private
        
        [Header("Prefabs")]
        [SerializeField] private GameObject vehiclePrefab;
        [SerializeField] private GameObject environmentPrefab;
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private GameObject starPrefab;
        
        [Header("Input Action")]
        [SerializeField] private InputActionReference moveAction;
        
        [Header("Spawn Settings")]
        [SerializeField] private int starsToSpawn = 10;
        
        [Header("Spawned Objects")]
        [SerializeField] private GameObject currentVehicle;
        [SerializeField] private GameObject currentEnvironment;
        
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
                PlayerController controller = FindObjectOfType<PlayerController>();
                if (controller != null)
                {
                    currentVehicle = controller.gameObject;
                }
            }

            if (currentEnvironment == null)
            {
                // Try to find by tag first
                try
                {
                    currentEnvironment = GameObject.FindWithTag("Environment");
                }
                catch
                {
                    Debug.LogWarning("Environment tag not defined. Please tag your street/environment GameObject with 'Environment' tag.");
                }
            }

            // Assign the move action to the current vehicle's PlayerController
            AssignMoveActionToCurrentVehicle();
        }

        private void AssignMoveActionToCurrentVehicle()
        {
            if (currentVehicle != null)
            {
                PlayerController controller = currentVehicle.GetComponent<PlayerController>();
                if (controller != null && moveAction != null)
                {
                    controller.SetMoveAction(moveAction);
                }
                else if (controller == null)
                {
                    Debug.LogError("Current vehicle does not have PlayerController component!");
                }
                else if (moveAction == null)
                {
                    Debug.LogError("Move Action is not assigned in VehicleManager!");
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
        
        public GameObject GetCurrentEnvironment()
        {
            return currentEnvironment;
        }
        
        public void SetCurrentEnvironment(GameObject environment)
        {
            currentEnvironment = environment;
        }
        
        public GameObject GetVehiclePrefab()
        {
            return vehiclePrefab;
        }
        
        public GameObject GetEnvironmentPrefab()
        {
            return environmentPrefab;
        }

        public GameObject GetObstaclePrefab()
        {
            return obstaclePrefab;
        }
        
        public GameObject GetStarPrefab()
        {
            return starPrefab;
        }

        public InputActionReference GetMoveAction()
        {
            return moveAction;
        }
    }
}