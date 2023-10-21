using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace Scripts.PowerUps {
    public class PowerUpManager : MonoBehaviour
    {
        private bool coroutineIsRunning;
        public static PowerUpManager powerUpManagerInstance;
        [SerializeField] private float spawnTimer;
        [SerializeField] private List<PowerUpScriptableObject> powerUpScriptableObjectsList;
        [SerializeField] private List<Transform> SpawnPoints;
        private void Start() {
            if (powerUpManagerInstance == null)
                powerUpManagerInstance = this;
        }

        private void Update() {
            if (!coroutineIsRunning) {
                StartCoroutine(SpawnRandomObjectCoroutine(spawnTimer));
            }
        }
        private IEnumerator SpawnRandomObjectCoroutine(float time) {
            coroutineIsRunning = true;
            yield return new WaitForSeconds(time);
            SpawnRandomPowerUp();
            coroutineIsRunning = false;

        }
        private void SpawnRandomPowerUp() {
            Transform powerUp = Instantiate(powerUpScriptableObjectsList[Random.Range(0,powerUpScriptableObjectsList.Count)].powerUpPrefab,SpawnPoints[Random.Range(0,SpawnPoints.Capacity)]);
            NetworkObjectManager.SpawningObjectServerRpc(powerUp);
        }
    }
}