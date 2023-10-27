using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour {
    private GameObject[] connectedPlayersGameObjects;

    RoundManager roundManager;
    public override void OnNetworkSpawn()
    {
        roundManager = GetComponent<RoundManager>();
    }
    private void Update() {
        connectedPlayersGameObjects = GameObject.FindGameObjectsWithTag("Player");
    }
}