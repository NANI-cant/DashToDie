using CodeBase.Gameplay.General.Fighting;
using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(MeleeAttacker))]
    [RequireComponent(typeof(Rotator))]
    public class SwordEnemyBrain: EnemyBrain {
        [SerializeField] [Min(0)] private float _aimAccuracy = 0.1f;
        
        private Transform _playerTarget;
        private MeleeAttacker _attacker;

        private bool _targetWasInRange;

        [Inject]
        public void Construct(GameObject player) {
            _playerTarget = player.transform;
        }
        
        protected override void Awake() {
            base.Awake();
            _attacker = GetComponent<MeleeAttacker>();
        }

        private void Update() {
            bool isTargetInRange = _attacker.CheckTargetInRange(_playerTarget);
            
            if(_attacker.IsCharged) return;
            
            if (!isTargetInRange) {
                if (_targetWasInRange) _attacker.Cancel();

                Rotator.Disable();
                Agent.isStopped = false;
                if (Agent.enabled) Agent.SetDestination(_playerTarget.position);
            }
            else {
                Agent.isStopped = true;
                Rotator.Enable();
                Rotator.RotateToPoint(_playerTarget.position);
                
                Vector3 directionToPlayer = transform.position.DirectionTo(_playerTarget.position);
                directionToPlayer.y = transform.forward.y;
                if (transform.forward.ApproximateEqual(directionToPlayer, _aimAccuracy)) {
                    _attacker.Attack();   
                }
            }

            _targetWasInRange = isTargetInRange;
        }
    }
}