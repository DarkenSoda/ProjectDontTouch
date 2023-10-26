using UnityEngine;
using Unity.Netcode;
using System;

namespace Scripts.PowerUps {
    public abstract class PowerUpBehaviour : NetworkBehaviour {
        public string powerUpName;
        public Sprite powerUpSprite;
        public int numberOfCasts;

        public PlayerPowerUp Player { set; protected get; }
        
        protected Action currentCast;

        public abstract void ApplyPowerUp();

        public abstract void DestroyPower();
    }
}