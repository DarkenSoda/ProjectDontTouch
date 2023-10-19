using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps")]
    
    public abstract class PowerUpScriptableObject : ScriptableObject
    {
        public string powerUpName;
        public Sprite powerUpSprite;
        public abstract void ApplyPowerUp();
    }
}