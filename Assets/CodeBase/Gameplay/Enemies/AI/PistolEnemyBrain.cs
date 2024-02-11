using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(PistolAttacker))]
    public class PistolEnemyBrain: EnemyBrain {
        [SerializeField] [Min(0)] private float _aimAccuracy = 0.1f;
        
        private Transform _playerTarget;
        private PistolAttacker _attacker;

        private bool _targetWasInRange;
        private float _delayRemaining;

        [Inject]
        public void Construct(GameObject player) {
            _playerTarget = player.transform;
        }
        
        protected override void Awake() {
            base.Awake();
            _attacker = GetComponent<PistolAttacker>();
        }

        private void Update() {
            bool isTargetInRange = _attacker.CheckTargetInRange(_playerTarget);
            
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