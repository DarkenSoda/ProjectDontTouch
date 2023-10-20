using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.PowerUps;
public class MovementSpeedPowerUp : PowerUpScriptableObject
{
    public float additionalMovementSpeed;
    public override void ApplyPowerUp(Transform player) {
        PlayerContext playerContext = player.GetComponent<PlayerContext>();
        //apply movement Speed;
    }
}
