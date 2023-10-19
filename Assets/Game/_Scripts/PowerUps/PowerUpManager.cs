using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace Scripts.PowerUps {
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager powerUpManagerInstance;
        [SerializeField] private List<PowerUpScriptableObject> TaggerpowerUpScriptableObjectsList;
        [SerializeField] private List<PowerUpScriptableObject> RunnerpowerUpScriptableObjectsList;
        [SerializeField] private List<Transform> SpawnPoints;
        private void Start() {
            if (powerUpManagerInstance == null)
                powerUpManagerInstance = this;
        }
        
    }
}