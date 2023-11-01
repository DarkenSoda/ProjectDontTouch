using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;


namespace Scripts.PowerUps {
    public class PowerUpManager : NetworkBehaviour {
        public static PowerUpManager Instance { get; private set; }
        [HideInInspector]
        public List<Transform> occupiedTaggersSpawnPoints;
        [HideInInspector]
        public List<Transform> occupiedRunnersSpawnPoints;
        [Header("PowerUp Manager References")]
        [SerializeField] private float spawnTimer;
        [SerializeField] private List<Transform> TaggerPowerUps;
        [SerializeField] private List<Transform> RunnerPowerUps;
        public List<Transform> RunnerSpawnPoints;
        public List<Transform> TaggerSpawnPoints;
        private void Start() {
            if (Instance == null) {
                Instance = this;
            }

            occupiedRunnersSpawnPoints = new();
            occupiedTaggersSpawnPoints = new();
        }
        public override void OnNetworkSpawn() {
            if (IsServer) {
                StartCoroutine(SpawnRandomObjectCoroutine());
            }
        }

        private IEnumerator SpawnRandomObjectCoroutine() {
            while (true) {
                if (TaggerSpawnPoints.Count != 0) {
                    SpawningPowerUp(PlayerRole.Tagger);
                }
                if (RunnerSpawnPoints.Count != 0) {
                    SpawningPowerUp(PlayerRole.Runner);
                }
                yield return new WaitForSeconds(spawnTimer);
            }
        }

        private void SpawningPowerUp(PlayerRole playerRole) {
            List<Transform> spawnPoints;
            List<Transform> occupiedSpawnPoints;
            List<Transform> powerUpList;
            switch (playerRole) {
                case PlayerRole.Tagger:
                    spawnPoints = TaggerSpawnPoints;
                    occupiedSpawnPoints = occupiedTaggersSpawnPoints;
                    powerUpList = TaggerPowerUps;
                    break;
                case PlayerRole.Runner:
                    spawnPoints = RunnerSpawnPoints;
                    occupiedSpawnPoints = occupiedRunnersSpawnPoints;
                    powerUpList = RunnerPowerUps;
                    break;
                default:
                    return;
            }

            Transform SpawnPointTransform = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Transform powerUpObject = powerUpList[Random.Range(0, powerUpList.Count)];
            Transform powerUp = Instantiate(powerUpObject, SpawnPointTransform.position, Quaternion.identity);
            powerUp.GetComponent<NetworkObject>().Spawn();
            
            powerUp.GetComponent<PowerUpBuff>().currentSpawnPoint = SpawnPointTransform;

            occupiedSpawnPoints.Add(SpawnPointTransform);
            spawnPoints.Remove(SpawnPointTransform);
        }
    }
}