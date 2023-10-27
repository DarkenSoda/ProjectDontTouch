using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Scripts.PowerUps {
    public class PowerUpManager : NetworkBehaviour {
        public static PowerUpManager Instance { get; private set; }
        private List<Transform> occupiedTaggersSpawnPoints;
        private List<Transform> occupiedRunnersSpawnPoints;
        private bool TaggerTurn;

        [Header("PowerUp Manager References")]
        [SerializeField] private float spawnTimer;
        [SerializeField] private List<Transform> TaggerPowerUps;
        [SerializeField] private List<Transform> RunnerPowerUps;
        [SerializeField] private List<Transform> RunnerSpawnPoints;
        [SerializeField] private List<Transform> TaggerSpawnPoints;
        private void Start() {
            TaggerTurn = true;
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
                if (TaggerTurn) {
                    SpawningTaggerPowerUp();
                }
                else {
                    SpawningRunnerPowerUp();
                }
                TaggerTurn = !TaggerTurn;
                yield return new WaitForSeconds(spawnTimer);
            }
        }

        private void SpawningTaggerPowerUp() {
            if (TaggerSpawnPoints.Count == 0) return;

            Transform taggerSpawnPoint = TaggerSpawnPoints[Random.Range(0,TaggerSpawnPoints.Count)];
            Transform powerUpObject = TaggerPowerUps[Random.Range(0, TaggerPowerUps.Count)];
            Transform powerUp = Instantiate(powerUpObject, taggerSpawnPoint.position, Quaternion.identity);
            powerUp.GetComponent<NetworkObject>().Spawn();
            
            occupiedTaggersSpawnPoints.Add(taggerSpawnPoint);
            TaggerSpawnPoints.Remove(taggerSpawnPoint);
            
            powerUpObject.GetComponent<PowerUpBuff>().currentSpawnPoint = taggerSpawnPoint;
        }

        private void SpawningRunnerPowerUp() {
            if (RunnerSpawnPoints.Count == 0) return;

            Transform runnerSpawnPoint = RunnerSpawnPoints[Random.Range(0, RunnerSpawnPoints.Count)];
            Transform powerUpObject = RunnerPowerUps[Random.Range(0, RunnerPowerUps.Count)];
            Transform powerUp = Instantiate(powerUpObject, runnerSpawnPoint.position, Quaternion.identity);
            powerUp.GetComponent<NetworkObject>().Spawn();

            occupiedRunnersSpawnPoints.Add(runnerSpawnPoint);
            RunnerSpawnPoints.Remove(runnerSpawnPoint);

            powerUpObject.GetComponent<PowerUpBuff>().currentSpawnPoint = runnerSpawnPoint;
        }

        public void RunnerSpawnPointsAddItem(Transform transform) {
            RunnerSpawnPoints.Add(transform);
        }

        public void TaggerSpawnPointsAddItem(Transform transform) {
            TaggerSpawnPoints.Add(transform);
        }
        
        public void OccupiedRunnerSpawnPointsAddItem(Transform transform) {
            occupiedRunnersSpawnPoints.Add(transform);
        }

        public void OccupiedTaggerSpawnPointsAddItem(Transform transform) {
            occupiedTaggersSpawnPoints.Add(transform);
        }
        public void RunnerSpawnPointsRemoveItem(Transform transform) {
            RunnerSpawnPoints.Remove(transform);
        }

        public void TaggerSpawnPointsRemoveItem(Transform transform) {
            TaggerSpawnPoints.Remove(transform);
        }
        public void OccupiedRunnerSpawnPointsRemoveItem(Transform transform) {
            occupiedRunnersSpawnPoints.Remove(transform);
        }

        public void OccupiedTaggerSpawnPointsRemoveItem(Transform transform) {
            occupiedTaggersSpawnPoints.Remove(transform);
        }
    }
}