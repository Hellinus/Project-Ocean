using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.InfiniteRunnerEngine;
using UnityEngine;

namespace _Ocean.Scripts.Managers
{
    public class GyroManager : MMSingleton<GyroManager>
    {
        public bool HasGyroscope => SystemInfo.supportsGyroscope;
        
        protected Vector3 BaseEulerAngles = Vector3.zero;
        protected Vector3 BaseRotationRate = Vector3.zero;
        protected bool _gyroInitialized = false;

        public Quaternion GetCurrentAttitude()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            return HasGyroscope ? ReadGyroscopeAttitude() : Quaternion.identity;
        }
        
        public Vector3 GetCurrentUnityEulerAngles()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            Vector3 v = ReadGyroscopeEulerAngles();
            v = new Vector3(v.x, -v.z + 180, v.y);
            
            return HasGyroscope ? v : Vector3.zero;
        }
        
        public Vector3 GetCurrentProcessedEulerAngles()
        {
            return HasGyroscope? GetCurrentUnityEulerAngles() - BaseEulerAngles : Vector3.zero;
        }

        public Vector3 GetCurrentUnityRotationRate()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            Vector3 v = ReadGyroscopeRotationRateUnbiased();
            v = new Vector3(-v.x, -v.z, -v.y);
            
            return HasGyroscope ? v : Vector3.zero;
        }
        
        public Vector3 GetCurrentProcessedRotationRate()
        {
            return HasGyroscope? GetCurrentUnityRotationRate() - BaseRotationRate : Vector3.zero;
        }

        public void SetCurrentBaseEulerAngles()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            if (!HasGyroscope) return;

            BaseEulerAngles = GetCurrentUnityEulerAngles();
        }
        
        public void SetCurrentBaseRotationRate()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            if (!HasGyroscope) return;

            BaseRotationRate = GetCurrentUnityRotationRate();
        }
        
        protected void InitGyro()
        {
            if (HasGyroscope)
            {
                Input.gyro.enabled = true;
                Input.gyro.updateInterval = 0.0167f; // 60HZ
            }

            _gyroInitialized = true;
        }

        protected Quaternion ReadGyroscopeAttitude()
        {
            return Input.gyro.attitude;
        }
        
        protected Vector3 ReadGyroscopeEulerAngles()
        {
            return Input.gyro.attitude.eulerAngles;
        }

        protected Vector3 ReadGyroscopeRotationRateUnbiased()
        {
            return Input.gyro.rotationRateUnbiased;
        }
    }
}

