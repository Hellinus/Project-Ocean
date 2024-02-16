using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _Ocean.Scripts.Player
{
    public enum AbilityButtonType
    {
        ForceLeft,
        ForceMiddle,
        ForceRight,
        NoLimit
    }
    
    public class Ability : MonoBehaviour
    {
        public string AbilityName;
        public float CoolDownTime = 1;
        public int CurrentAbilityLevel = 1;
        public int MaxAbilityLevel = 3;
        public int CanUseTimes = -1;
        public AbilityButtonType CurrentButtonType = AbilityButtonType.NoLimit;


        protected float _initialCoolDownTime;
        protected int _initialAbilityLevel;
        protected float _currentCoolDownTime;
        
        protected virtual void Start()
        {
            _initialCoolDownTime = CoolDownTime;
            _initialAbilityLevel = CurrentAbilityLevel;
        }

        public virtual void AbilityButtonDown()
        {
            
        }
        
        public virtual void AbilityButtonUp()
        {
            
        }
        
        public virtual void AbilityButtonPressed()
        {
            
        }
        
        protected virtual void AbilityUseTimesZero()
        {
            
        }

        public virtual void SetCoolDownTime(float targetTime)
        {
            CoolDownTime = targetTime;
        }
        
        public virtual void ResetCoolDownTime()
        {
            CoolDownTime = _initialCoolDownTime;
        }
        
        public virtual void TemporarilyAddCoolDownTime(float percent)
        {
            
        }
        
        public virtual void SetAbilityLevel(int targetLevel)
        {
            CurrentAbilityLevel = targetLevel;
        }
        
        public virtual void ResetAbilityLevel()
        {
            CurrentAbilityLevel = _initialAbilityLevel;
        }
        
        public virtual void SetCanUseTimes(int targetTimes)
        {
            CanUseTimes = targetTimes;
        }
    }
}
