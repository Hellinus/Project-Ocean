using System.Collections;
using System.Collections.Generic;
using _Ocean.Scripts.Managers;
using MoreMountains.Tools;
using MoreMountains.InfiniteRunnerEngine;
using UnityEngine;

namespace _Ocean.Scripts.Player
{
    public class ShipCharacter : PlayableCharacter
    {
        [Header("Movement")]
        /// the character's movement speed
        public float MoveSpeed = 2f;
        /// the movement inertia (the higher it is, the longer it takes for it to stop / change direction
        public float MovementInertia = 1f;
        /// if true, the character will stop before reaching the level's death bounds
        public bool ConstrainMovementToDeathBounds=true;
        [Range(0f, 120f)]
        public float XMovementLimit = 10f;
        [Range(0f, 120f)]
        public float YMovementLimit = 10f;
        [Range(0f, 120f)]
        public float ZMovementLimit = 10f;

        [Header("Rotation")]
        public float RotateInertia = 1f;
        [Range(0f, 120f)]
        public float XRotationLimit = 30f;
        [Range(0f, 120f)]
        public float YRotationLimit = 30f;
        [Range(0f, 120f)]
        public float ZRotationLimit = 30f;
        public float ReturnTime = 2f;
        
        protected float _minBound_x;
        protected float _maxBound_x;
        protected float _minBound_z;
        protected float _maxBound_z;
        protected Vector3 _newPosition = Vector3.zero;
        protected Vector3 _movement = Vector3.zero;
        protected Vector3 _currentMovement = Vector3.zero;
        
        
        protected float _boundsSecurity = 10f;
        protected bool _isReturning = false;
        protected float _curTime = 0f;
        protected Vector3 _preEulerAngles = Vector3.zero;
        protected Vector3 _currentRotationRate = Vector3.zero;
        protected float _minRotation_x;
        protected float _minRotationBound_x;
        protected float _maxRotation_x;
        protected float _maxRotationBound_x;
        protected float _minRotation_y;
        protected float _minRotationBound_y;
        protected float _maxRotation_y;
        protected float _maxRotationBound_y;
        protected float _minRotation_z;
        protected float _minRotationBound_z;
        protected float _maxRotation_z;
        protected float _maxRotationBound_z;
        
        
        /// <summary>
        /// On start we set bounds and movement axis based on what's been set in the inspector
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _minBound_x = LevelManager.Instance.transform.position.x + LevelManager.Instance.DeathBounds.center.x - LevelManager.Instance.DeathBounds.extents.x + _boundsSecurity;
            _maxBound_x = LevelManager.Instance.transform.position.x + LevelManager.Instance.DeathBounds.center.x + LevelManager.Instance.DeathBounds.extents.x - _boundsSecurity;
            _minBound_z = LevelManager.Instance.transform.position.z + LevelManager.Instance.DeathBounds.center.z - LevelManager.Instance.DeathBounds.extents.z + _boundsSecurity;
            _maxBound_z = LevelManager.Instance.transform.position.z + LevelManager.Instance.DeathBounds.center.z + LevelManager.Instance.DeathBounds.extents.z - _boundsSecurity;

            _minRotation_x = 360 - XRotationLimit;
            _minRotationBound_x = _minRotation_x - 60;
            _maxRotation_x = XRotationLimit;
            _maxRotationBound_x = _maxRotation_x + 60;
            _minRotation_y = 360 - YRotationLimit;
            _minRotationBound_y = _minRotation_y - 60;
            _maxRotation_y = YRotationLimit;
            _maxRotationBound_y = _maxRotation_y + 60;
            _minRotation_z = 360 - ZRotationLimit;
            _minRotationBound_z = _minRotation_z - 60;
            _maxRotation_z = ZRotationLimit;
            _maxRotationBound_z = _maxRotation_z + 60;

            GyroManager.Instance.SetCurrentBaseEulerAngles();
        }
        
        /// <summary>
        /// On update we just handle our character's movement
        /// </summary>
        protected override void Update ()
        {
            base.Update();
            MoveCharacter();
            RotateCharacter();
        }

        /// <summary>
        /// Every frame, we move our character
        /// </summary>
        protected virtual void MoveCharacter()
        {
            if (_isReturning)
            {
                _movement = new Vector3(0, 0, 0);
                _currentMovement = new Vector3(0, 0, 0);
                _newPosition = new Vector3(0, 0, 0);
            }
            else
            {
                Vector3 v = GyroManager.Instance.GetInspectorRotationValueMethod(this.transform);

                if (v.x < -XMovementLimit) v.x = -XMovementLimit;
                if (v.x > XMovementLimit) v.x = XMovementLimit;
                // if (v.y < -YMovementLimit) v.y = -YMovementLimit;
                // if (v.y > YMovementLimit) v.y = YMovementLimit;
                if (v.z < -ZMovementLimit) v.z = -ZMovementLimit;
                if (v.z > ZMovementLimit) v.z = ZMovementLimit;
                if (Mathf.Abs(v.x) is > -1 and < 1) v.x = 0;
                // if (Mathf.Abs(v.y) is > -1 and < 1) v.y = 0;
                if (Mathf.Abs(v.z) is > -1 and < 1) v.z = 0;
                
                _movement = new Vector3(-v.z, 0, v.x);
                Debug.Log("movement:" + _movement);
                _currentMovement = Vector3.Lerp(_currentMovement,_movement,Time.deltaTime * 1/MovementInertia);
                _newPosition = transform.position + _currentMovement * MoveSpeed;
                _newPosition = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime);
                if (ConstrainMovementToDeathBounds)
                {
                    _newPosition.x = Mathf.Clamp(_newPosition.x,_minBound_x,_maxBound_x);
                    _newPosition.z = Mathf.Clamp(_newPosition.z,_minBound_z,_maxBound_z);
                }
                
                transform.position = _newPosition;

            }
        }

        /// <summary>
        /// Every frame, we rotate our character
        /// </summary>
        protected virtual void RotateCharacter()
        {
            if (_isReturning)
            {
                GyroManager.Instance.SetCurrentBaseEulerAngles();
                transform.eulerAngles = MMTween.Tween(_curTime, 0f, ReturnTime, 
                    _preEulerAngles, GyroManager.Instance.GetCurrentProcessedEulerAngles(),
                    MMTween.MMTweenCurve.EaseInOutQuadratic);
                _curTime += Time.deltaTime;

                if (_curTime >= ReturnTime)
                {
                    _isReturning = false;
                    _curTime = 0f;
                    _currentRotationRate = Vector3.zero;
                }
            }
            else
            {
                Vector3 eulerAngles = transform.eulerAngles;
                Vector3 v = GyroManager.Instance.GetCurrentUnityRotationRate();
                if (((eulerAngles.x <= _minRotation_x && eulerAngles.x > _minRotationBound_x) && v.x < 0)
                    || ((eulerAngles.x >= _maxRotation_x && eulerAngles.x < _maxRotationBound_x) && v.x > 0))
                {
                    v.x = 0;
                }
                if (((eulerAngles.y <= _minRotation_y && eulerAngles.y > _minRotationBound_y) && v.y < 0)
                    || ((eulerAngles.y >= _maxRotation_y && eulerAngles.y < _maxRotationBound_y) && v.y > 0))
                {
                    v.y = 0;
                } 
                if (((eulerAngles.z <= _minRotation_z && eulerAngles.z > _minRotationBound_z) && v.z < 0)
                    || ((eulerAngles.z >= _maxRotation_z && eulerAngles.z < _maxRotationBound_z) && v.z > 0))
                {
                    v.z = 0;
                }

                _currentRotationRate = Vector3.Lerp(_currentRotationRate, v, Time.deltaTime * 1/RotateInertia);
                transform.Rotate(_currentRotationRate);
            }
        }
        
        public override void MainActionStart()
        {
            base.MainActionStart();
            if(_isReturning) return;

            _preEulerAngles = this.transform.eulerAngles;
            _isReturning = true;
        }
    }
}

