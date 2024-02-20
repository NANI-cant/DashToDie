using CodeBase.Gameplay.General.Fighting;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(RifleAttacker))]
    public class RifleEnemyBrain: EnemyBrain {
        [SerializeField] [Min(0)] private float _aimAccuracy = 0.1f;
        
        private readonly int _movementSpeedKey = Animator.StringToHash("MovementSpeed");
        
        private Transform _playerTarget;
        private RifleAttacker _attacker;

        [Inject]
        public void Construct(GameObject player) {
            _playerTarget = player.transform;
        }
        
        protected override void Awake() {
            base.Awake();
            _attacker = GetComponent<RifleAttacker>();
        }

        private void Update() {
            if (CheckApproachedTarget(_playerTarget) && CanSeeTarget(_playerTarget)) {
                Agent.isStopped = true;
                _animator.SetFloat(_movementSpeedKey, 0);
                Rotator.Enable();

                if (_attacker.IsReloading) return;
                
                Rotator.RotateToPoint(_playerTarget.position);

                Vector3 directionToPlayer = Transform.position.DirectionTo(_playerTarget.position);
                directionToPlayer.y = transform.forward.y;
                directionToPlayer.Normalize();
                if (transform.forward.ApproximateEqual(directionToPlayer, _aimAccuracy)) {
                    _attacker.Attack();
                }
                
                if (_attacker.Ammo.Current == 0) _attacker.Reload();
            }
            else {
                if(_attacker.IsCharging || _attacker.IsReloading) return;

                Rotator.Disable();
                _attacker.Cancel();
                Agent.isStopped = false;
                _animator.SetFloat(_movementSpeedKey, 1);
                if (Agent != null) Agent.SetDestination(_playerTarget.position);
            }
        }
    }
}