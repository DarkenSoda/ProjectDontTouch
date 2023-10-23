using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace Scripts.PowerUps {
    public class PowerUpManager : NetworkBehaviour
    {
        public static PowerUpManager powerUpManagerInstance;
        [SerializeField] private float spawnTimer;
        [SerializeField] private List<PowerUpScriptableObject> powerUpScriptableObjectsList;
        [SerializeField] private List<Transform> SpawnPoints;

        private List<Transform> availableSpawnPoints;
        
        private void Start() {
            if (powerUpManagerInstance == null)
                powerUpManagerInstance = this;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer) {
                StartCoroutine(SpawnRandomObjectCoroutine(spawnTimer));
            }
        }

        private IEnumerator SpawnRandomObjectCoroutine(float time) {
            while (true) {
                SpawningPowerUp();
                yield return new WaitForSeconds(time);
            }

        }
       
        private void SpawningPowerUp() {
            Transform randomSpawnPoint = availableSpawnPoints[Random.Range(0,availableSpawnPoints.Capacity)];
            Transform powerUp = Instantiate(powerUpScriptableObjectsList[Random.Range(0,powerUpScriptableObjectsList.Count)].powerUpPrefab,randomSpawnPoint.position,Quaternion.identity);
            powerUp.GetComponent<NetworkObject>().Spawn();
            availableSpawnPoints.Remove(randomSpawnPoint);
        }
    }
}