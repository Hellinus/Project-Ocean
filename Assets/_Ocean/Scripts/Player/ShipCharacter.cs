using UnityEngine;
using MoreMountains.InfiniteRunnerEngine;

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
            var rot = Vector3.zero;
            // rot.z = -Input.gyro.rotationRateUnbiased.y;
            // rot.z = -Input.gyro.attitude.y;
            // transform.Rotate(rot);
            // Quaternion de = new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
            // transform.rotation = de;
            Vector3 a = Input.gyro.attitude.eulerAngles;
            a = new Vector3(-a.x, -a.z, -a.y); //正确的方向转换
            this.transform.eulerAngles = a;
            // this.transform.Rotate(Vector3.right * 90, Space.World);
            Debug.Log(Input.gyro.attitude);
        }

        
    }
}

