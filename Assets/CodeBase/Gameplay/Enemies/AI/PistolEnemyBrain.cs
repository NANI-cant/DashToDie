using CodeBase.Gameplay.General.Fighting;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(PistolAttacker))]
    public class PistolEnemyBrain: EnemyBrain {
        [SerializeField] [Min(0)] private float _aimAccuracy = 0.1f;
        
        private readonly int _movementSpeedKey = Animator.StringToHash("MovementSpeed");
        
        private Transform _playerTarget;
        private PistolAttacker _attacker;

        [Inject]
        public void Construct(GameObject player) {
            _playerTarget = player.transform;
        }
        
        protected override void Awake() {
            base.Awake();
            _attacker = GetComponent<PistolAttacker>();
        }

        private void Update() {
            if (CheckApproachedTarget(_playerTarget) && CanSeeTarget(_playerTarget)) {
                Agent.isStopped = true;
                _animator.SetFloat(_movementSpeedKey, 0);
                Rotator.Enable();
                Rotator.RotateToPoint(_playerTarget.position);

                Vector3 directionToPlayer = Transform.position.DirectionTo(_playerTarget.position);
                directionToPlayer.y = transform.forward.y;
                directionToPlayer.Normalize();

                if (transform.forward.ApproximateEqual(directionToPlayer, _aimAccuracy)) {
                    _attacker.Attack();    
                }
            }
            else {
                if(_attacker.IsCharging) return;
                
                Rotator.Disable();
                _attacker.Cancel();
                Agent.isStopped = false;
                _animator.SetFloat(_movementSpeedKey, 1);
                if (Agent != null) Agent.SetDestination(_playerTarget.position);
            }
        }
    }
}