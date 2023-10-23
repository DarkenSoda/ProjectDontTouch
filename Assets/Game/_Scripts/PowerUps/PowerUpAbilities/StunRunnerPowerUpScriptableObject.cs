using System.Collections;
using System.Collections.Generic;
using Scripts.PowerUps;
using UnityEngine;

public class StunRunnerPowerUpScriptableObject : PowerUpScriptableObject
{
    public override void ApplyPowerUp(Transform playerTransform, int castNumber)
    {
        switch(castNumber) {
            case 1:
                StunFirstCast(playerTransform);
                break;
        }
    }

    private void StunFirstCast(Transform playerTransform) {
        //Implement Later.
    }
}
