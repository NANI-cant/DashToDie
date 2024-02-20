using CodeBase.Gameplay.General.Fighting;
using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(MeleeAttacker))]
    public class SwordEnemyBrain: EnemyBrain {
        private Transform _playerTarget;
        private MeleeAttacker _attacker;
        
        private readonly int _movementSpeedKey = Animator.StringToHash("MovementSpeed");

        [Inject]
        public void Construct(GameObject player) {
            _playerTarget = player.transform;
        }
        
        protected override void Awake() {
            base.Awake();
            _attacker = GetComponent<MeleeAttacker>();
        }

        private void Update() {
            if (CheckApproachedTarget(_playerTarget) && CanSeeTarget(_playerTarget)) {
                Agent.isStopped = true;
                _animator.SetFloat(_movementSpeedKey, 0);
                Rotator.Enable();
                Rotator.RotateToPoint(_playerTarget.position);
                _attacker.Attack();
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