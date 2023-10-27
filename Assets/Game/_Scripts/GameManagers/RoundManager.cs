using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class RoundManager : NetworkBehaviour
{
    private int currentRound;
    private float roundTimer;
    [Header("Round References")]
    [SerializeField] private int maxRounds;
    [SerializeField] private float maxRoundTime;

    public Action RoundStartAction;
    public Action RoundEndAction;
    public Action GameEndAction;
    public override void OnNetworkSpawn()
    {
        roundTimer = 0f;
        currentRound = 1;

    }

}
