using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MoreMountains.Tools;
using MoreMountains.InfiniteRunnerEngine;
using UnityEngine;

namespace _Ocean.Scripts.Managers
{
    public class GyroManager : MMSingleton<GyroManager>
    {
        public bool HasGyroscope => SystemInfo.supportsGyroscope;
        
        protected Vector3 _baseEulerAngles = Vector3.zero;
        protected bool _gyroInitialized = false;

        public Quaternion GetCurrentAttitude()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            return HasGyroscope ? ReadGyroscopeAttitude() : Quaternion.identity;
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

        #region EulerAngles

        public Vector3 GetBaseEulerAngles()
        {
            return _baseEulerAngles;
        }
        
        public Vector3 GetCurrentUnityEulerAngles()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            Vector3 v = ReadGyroscopeEulerAngles();
            v = new Vector3(v.x, -v.z + 180, -v.y);
            
            return HasGyroscope ? v : Vector3.zero;
        }
        
        public Vector3 GetCurrentProcessedEulerAngles()
        {
            Vector3 temp = GetCurrentUnityEulerAngles();
            return HasGyroscope? new Vector3(temp.x - _baseEulerAngles.x, temp.y - _baseEulerAngles.y, temp.z - _baseEulerAngles.z)
                : Vector3.zero;
        }
        
        public void SetCurrentBaseEulerAngles()
        {
            if (!_gyroInitialized)
            {
                InitGyro();
            }

            if (!HasGyroscope) return;
            
            _baseEulerAngles = GetCurrentUnityEulerAngles();
        }

        #endregion
        
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
        
        //获取到旋转的正确数值
        public Vector3 GetInspectorRotationValueMethod(Transform transform)
        {
            // 获取原生值
            System.Type transformType = transform.GetType();
            PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
            object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
            MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
            string temp = value.ToString();
            //将字符串第一个和最后一个去掉
            temp = temp.Remove(0, 1);
            temp = temp.Remove(temp.Length - 1, 1);
            //用‘，’号分割
            string[] tempVector3;
            tempVector3 = temp.Split(',');
            //将分割好的数据传给Vector3
            Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
            return vector3;
        }
    }
}

