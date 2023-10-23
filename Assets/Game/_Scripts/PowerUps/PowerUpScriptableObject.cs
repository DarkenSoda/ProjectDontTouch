using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.PowerUps
{    
    public abstract class PowerUpScriptableObject : ScriptableObject
    {
        public string powerUpName;
        public Sprite powerUpSprite;
        public Transform powerUpPrefab;
        public int numberOfCasts;
        public PlayerPowerUpSystem.PlayerPowerUp powerUpState;
        public abstract void ApplyPowerUp(Transform playerTransform, int castNumber);
    }
}