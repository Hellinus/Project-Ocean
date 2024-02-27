using System.Collections;
using System.Collections.Generic;
using _Ocean.Scripts.Managers;
using MoreMountains.Tools;
using MoreMountains.InfiniteRunnerEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Ocean.Scripts.Player
{
    public class ShipCharacter : PlayableCharacter
    {
        [Header("Movement")]
        /// the character's movement speed
        public float MaxSpeed = 30f;
        public float MoveForce = 2f;
        /// the movement inertia (the higher it is, the longer it takes for it to stop / change direction
        public float MovementDrag = 1f;
        [Range(0f, 120f)]
        public float XMovementLimit = 10f; // 角度10度
        [Range(0f, 120f)]
        public float YMovementLimit = 10f;
        [Range(0f, 120f)]
        public float ZMovementLimit = 10f;

        [Header("Rotation")]
        public float RotateInertia = 0.1f;
        public float ReturnTime = 1.5f;
        public float ReturnCoolDownTime = 2f;
        [Range(0f, 120f)]
        public float XRotationLimit = 30f; // 船可以旋转多少度
        [Range(0f, 120f)]
        public float YRotationLimit = 30f;
        [Range(0f, 120f)]
        public float ZRotationLimit = 30f;

        
        protected Vector3 _direction = Vector3.zero;
        protected Rigidbody _rigidbody;
        

        protected bool _isReturning = false;
        protected float _curTime = 0f;
        protected Vector3 _preEulerAngles = Vector3.zero;
        protected Vector3 _currentRotationRate = Vector3.zero;
        protected float _curReturnCoolDownTime = 0f;
        protected bool _canReturn = false;
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
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Need Rigidbody.");
            }

            _rigidbody.drag = MovementDrag;
            
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

            AilityInitiate();
        }

        protected void AilityInitiate()
        {
            
            if (MaxAbilitiesNum != 0)
            {
                for (int i = 0; i < MaxAbilitiesNum; i++)
                {
                    CurrentAbilities.Add(null);
                }

                Ability[] l = GetComponents<Ability>();
                
                for (int i = 0; i < MaxAbilitiesNum; i++)
                {
                    if (i <= l.Length - 1)
                    {
                        switch (l[i].CurrentButtonType)
                        {
                            case AbilityButtonType.ForceLeft:
                                if (CurrentAbilities[0] == null)
                                {
                                    CurrentAbilities.Insert(0, l[i]);
                                    CurrentAbilities.RemoveAt(1);
                                }
                                else
                                {
                                    Debug.LogError("Ability type wrong: 2 ForceLeft exist.");
                                }
                                break;
                            case AbilityButtonType.ForceMiddle:
                                if (CurrentAbilities[1] == null)
                                {
                                    CurrentAbilities.Insert(1, l[i]);
                                    CurrentAbilities.RemoveAt(2);
                                }
                                else
                                {
                                    Debug.LogError("Ability type wrong: 2 ForceMiddle exist.");
                                }
                                break;
                            case AbilityButtonType.ForceRight:
                                if (CurrentAbilities[2] == null)
                                {
                                    CurrentAbilities.Insert(2, l[i]);
                                    CurrentAbilities.RemoveAt(3);
                                }
                                else
                                {
                                    Debug.LogError("Ability type wrong: 2 ForceRight exist.");
                                }
                                break;
                            case AbilityButtonType.NoLimit:
                                for (int j = 0; j < MaxAbilitiesNum; j++)
                                {
                                    if (CurrentAbilities[j] == null)
                                    {
                                        CurrentAbilities.Insert(j, l[i]);
                                        CurrentAbilities.RemoveAt(j + 1);
                                    }
                                    else
                                    {
                                        Debug.LogError("Ability type wrong: No spaces for abilities.");
                                    }
                                }
                                break;
                        }
                    }
                }
            }
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
                _direction = new Vector3(0, 0, 0);
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
                
                _direction = new Vector3(-v.z, 0, v.x);
                // Debug.Log("direction: " + _direction);
                
                _rigidbody.AddForce(_direction * MoveForce);
                
                _rigidbody.velocity = new Vector3(
                        Mathf.Clamp(_rigidbody.velocity.x,-MaxSpeed, MaxSpeed),
                        Mathf.Clamp(_rigidbody.velocity.y, -MaxSpeed, MaxSpeed),
                        Mathf.Clamp(_rigidbody.velocity.z, -MaxSpeed, MaxSpeed));
                _rigidbody.angularVelocity = new Vector3(0, 0, 0);
                
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

                // return finish
                if (_curTime >= ReturnTime)
                {
                    _isReturning = false;
                    _curTime = 0f;
                    _currentRotationRate = Vector3.zero;
                    _curReturnCoolDownTime = 0f; // reset timer
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
                
                if (_curReturnCoolDownTime >= ReturnCoolDownTime)
                {
                    _canReturn = true;
                }
                else
                {
                    _curReturnCoolDownTime += Time.deltaTime;
                }
            }
        }
        
        public override void MainActionStart()
        {
            base.MainActionStart();
            if(_isReturning) return;
            if (!_canReturn) return;

            _preEulerAngles = this.transform.eulerAngles;
            _isReturning = true;
            _canReturn = false;
        }

        public override void Ability1ButtonDown()
        {
            base.Ability1ButtonDown();
            if (CurrentAbilities[0] != null)
            {
                CurrentAbilities[0].AbilityButtonDown();
            }
        }
        
        public override void Ability1ButtonUp()
        {
            base.Ability1ButtonUp();
            if (CurrentAbilities[0] != null)
            {
                CurrentAbilities[0].AbilityButtonUp();
            }
        }
        
        public override void Ability1ButtonPressed()
        {
            base.Ability1ButtonPressed();
            if (CurrentAbilities[0] != null)
            {
                CurrentAbilities[0].AbilityButtonPressed();
            }
        }
        
        public override void Ability2ButtonDown()
        {
            base.Ability2ButtonDown();
            if (CurrentAbilities[1] != null)
            {
                CurrentAbilities[1].AbilityButtonDown();
            }
        }
        
        public override void Ability2ButtonUp()
        {
            base.Ability2ButtonUp();
            if (CurrentAbilities[1] != null)
            {
                CurrentAbilities[1].AbilityButtonUp();
            }
        }
        
        public override void Ability2ButtonPressed()
        {
            base.Ability2ButtonPressed();
            if (CurrentAbilities[1] != null)
            {
                CurrentAbilities[1].AbilityButtonPressed();
            }
        }
        
        public override void Ability3ButtonDown()
        {
            base.Ability3ButtonDown();
            if (CurrentAbilities[2] != null)
            {
                CurrentAbilities[2].AbilityButtonDown();
            }
        }
        
        public override void Ability3ButtonUp()
        {
            base.Ability3ButtonUp();
            if (CurrentAbilities[2] != null)
            {
                CurrentAbilities[2].AbilityButtonUp();
            }
        }
        
        public override void Ability3ButtonPressed()
        {
            base.Ability3ButtonPressed();
            if (CurrentAbilities[2] != null)
            {
                CurrentAbilities[2].AbilityButtonPressed();
            }
        }
        
    }
}

