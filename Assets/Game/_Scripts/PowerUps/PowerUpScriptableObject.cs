using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.PowerUps {
    public abstract class PowerUpScriptableObject : ScriptableObject {
        public string powerUpName;
        public Sprite powerUpSprite;
        public int numberOfCasts;
        public abstract void ApplyPowerUp(Transform playerTransform, int castNumber);
    }
}