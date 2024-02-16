using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Ocean.Scripts.Player
{
    public class AbilityRightCannon : Ability
    {
        protected bool _btnDown = false;
        
        public override void AbilityButtonDown()
        {
            base.AbilityButtonDown();
            if (_currentCoolDownTime <= CoolDownTime) return;

            Debug.Log("Right: Show trace.");
            _btnDown = true;
        }

        public override void AbilityButtonUp()
        {
            base.AbilityButtonUp();
            if (!_btnDown) return;
            
            Debug.Log("Right: Fire!");
            _currentCoolDownTime = 0;

            _btnDown = false;
        }
        
        protected void Update()
        {
            if (_currentCoolDownTime <= CoolDownTime)
            {
                _currentCoolDownTime += Time.deltaTime;
            }
        }
    }
}