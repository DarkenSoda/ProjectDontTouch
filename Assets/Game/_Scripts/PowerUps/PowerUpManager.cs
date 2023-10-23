using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Scripts.PowerUps {
    public class PowerUpManager : NetworkBehaviour {
        [SerializeField] private float spawnTimer;
        [SerializeField] private List<Transform> PowerUps;
        [SerializeField] private List<Transform> SpawnPoints;

        private List<Transform> availableSpawnPoints;

        public override void OnNetworkSpawn() {
            if (IsServer) {
                StartCoroutine(SpawnRandomObjectCoroutine());
            }
        }

        private IEnumerator SpawnRandomObjectCoroutine() {
            while (true) {
                SpawningPowerUp();
                yield return new WaitForSeconds(spawnTimer);
            }
        }

        private void SpawningPowerUp() {
            Transform powerUp = Instantiate(PowerUps[Random.Range(0, PowerUps.Count)],
                                SpawnPoints[Random.Range(0,SpawnPoints.Count)].position, Quaternion.identity);
            powerUp.GetComponent<NetworkObject>().Spawn();
        }
    }
}