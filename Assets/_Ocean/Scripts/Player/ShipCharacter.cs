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
        /// the character's movement speed
        public float MoveSpeed=5f;
        /// the movement inertia (the higher it is, the longer it takes for it to stop / change direction
        public float MovementInertia;
        /// if true, the character will stop before reaching the level's death bounds
        public bool ConstrainMovementToDeathBounds=true;

        protected float _minBound_x;
        protected float _maxBound_x;
        protected float _minBound_z;
        protected float _maxBound_z;
        protected float _boundsSecurity = 10f;
        
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

            Input.gyro.enabled = true;
        }
        
        /// <summary>
        /// On update we just handle our character's movement
        /// </summary>
        protected override void Update ()
        {
            base.Update();
            MoveCharacter();
        }

        /// <summary>
        /// Every frame, we move our character
        /// </summary>
        protected virtual void MoveCharacter()
        {
            Vector3 a = GyroManager.Instance.GetCurrentProcessedEulerAngles();
            this.transform.eulerAngles = a;
            
            // Vector3 a = GyroManager.Instance.GetCurrentProcessedRotationRate();
            // this.transform.Rotate(a);
        }

        public override void MainActionStart()
        {
            base.MainActionStart();
            GyroManager.Instance.SetCurrentBaseEulerAngles();
            GyroManager.Instance.SetCurrentBaseRotationRate();
        }
    }
}

