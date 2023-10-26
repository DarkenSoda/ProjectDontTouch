using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Reflection;

namespace Scripts.PowerUps {
    public abstract class PowerUpBehaviour : NetworkBehaviour, ICloneable{
        public string powerUpName;
        public Sprite powerUpSprite;
        public int numberOfCasts;
        public abstract void ApplyPowerUp(Transform playerTransform, int castNumber);

        public object Clone()
        {
            //we create a new instance of this specific type.            
            object newInstance = Activator.CreateInstance(this.GetType());

            //We get the array of properties for the new type instance.            
            FieldInfo[] fields = newInstance.GetType().GetFields();

            int i = 0;

            foreach (FieldInfo fi in this.GetType().GetFields())
            {
            fields[i].SetValue(newInstance, fi.GetValue(this));
            i++;
            }

            return newInstance;
        }
    }
}