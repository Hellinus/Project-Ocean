using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace _Ocean.Scripts.Player
{
    public class AbilityFrontCannon : Ability
    {
        public MMFeedbacks ShowTraceFeedbacks;
        public MMFeedbacks FireFeedBacks;
        
        protected bool _btnDown = false;
        
        
        public override void AbilityButtonDown()
        {
            base.AbilityButtonDown();
            if (_currentCoolDownTime <= CoolDownTime) return;
            
            Debug.Log("Front: Show trace.");
            ShowTraceFeedbacks?.PlayFeedbacks();
            _btnDown = true;
        }

        public override void AbilityButtonUp()
        {
            base.AbilityButtonUp();
            if (!_btnDown) return;
            
            Debug.Log("Front: Fire!");
            FireFeedBacks?.PlayFeedbacks();
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
