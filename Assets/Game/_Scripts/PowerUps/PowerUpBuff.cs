using System.Collections;
using System.Collections.Generic;
using Scripts.PowerUps;
using UnityEngine;

public class PowerUpBuff : MonoBehaviour
{
    [SerializeField] private PowerUpScriptableObject powerUpScriptableObject;

    public PowerUpScriptableObject GetPowerUpScriptableObject() {
        return powerUpScriptableObject;
    }
}
