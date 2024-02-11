using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Gameplay.General;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    public class MeleeAttacker: MonoBehaviour, IAttacker {
        [SerializeField][Min(0.0000001f)] private float _speed;
        [SerializeField][Min(0)] private int _damage;
        [SerializeField][Min(0.0000001f)] private float _delay = 0.1f;
        [SerializeField] private Vector3 _size;
        [SerializeField] private LayerMask _enemy;
        [SerializeField] private LayerMask _obstacle;
            
        [SerializeField] private Transform _centerOrigin;
        [SerializeField] private Transform _attackOrigin;

        private ITimeProvider _timeProvider;
        private float _lastAttackTime = float.NegativeInfinity;
        private CancellationTokenSource _attackCancel;

        public bool IsCharged => _attackCancel != null;

        private bool IsAttackCooldownPassed => (_timeProvider.Time - _lastAttackTime) > 1 / _speed;

        [Inject]
        public void Construct(ITimeProvider timeProvider) {
            _timeProvider = timeProvider;
        }

        private void OnDestroy() {
            Cancel();
        }

        public void Attack() {
            if(!IsAttackCooldownPassed || IsCharged) return;

            _attackCancel = new CancellationTokenSource();
            ExecuteAttack().Forget();
        }

        public void Cancel() {
            _attackCancel?.Cancel();
        }

        public bool CheckTargetInRange(Transform target) {
            List<GameObject> enemies = new List<GameObject>(CollectEnemiesInRange());
            return enemies.Contains(target.gameObject);
        }

        private async UniTaskVoid ExecuteAttack() {
            try {
                await UniTask.Delay((int) (_delay * 1000f), false, PlayerLoopTiming.Update, _attackCancel.Token);

                IHurtable[] targets = FindHurtablesInRange();
                if (targets.Length == 0) return;

                foreach (var target in targets)
                    target.TakeHit(_damage, out _);

                _lastAttackTime = _timeProvider.Time;
            }
            catch (OperationCanceledException) {
                
            }
            finally {
                _attackCancel?.Dispose();
                _attackCancel = null;
            }
        }

        private IHurtable[] FindHurtablesInRange() {
            var enemies = CollectEnemiesInRange();

            List<IHurtable> hurtables = new List<IHurtable>();
            foreach (var possibleHurtable in enemies) {
                if(!possibleHurtable.TryGetComponent<IHurtable>(out var hurtable)) continue;
                hurtables.Add(hurtable);
            }

            return hurtables.ToArray();
        }

        private GameObject[] CollectEnemiesInRange() {
            Collider[] possibleHurtables = Physics.OverlapBox(_attackOrigin.position, _size / 2, _attackOrigin.rotation, _enemy, QueryTriggerInteraction.Collide);
            
            List<GameObject> hurtableObjects = new();
            foreach (var hurtableCollider in possibleHurtables) {
                var hurtableObject = hurtableCollider.gameObject;
                
                var centerPosition = _centerOrigin.position;
                var hurtablePosition = hurtableObject.transform.position;
                
                float distance = Vector3.Distance(centerPosition, hurtablePosition);
                var colliderBounds = hurtableCollider.bounds;
                Ray rayLegs = new Ray(centerPosition, centerPosition.DirectionTo(colliderBounds.center + Vector3.down * colliderBounds.extents.y));
                Ray rayCenter = new Ray(centerPosition, centerPosition.DirectionTo(colliderBounds.center));
                Ray rayHead = new Ray(centerPosition, centerPosition.DirectionTo(colliderBounds.center + Vector3.up * colliderBounds.extents.y));

                if (!Physics.Raycast(rayLegs, distance, _obstacle, QueryTriggerInteraction.Ignore)
                    || !Physics.Raycast(rayCenter, distance, _obstacle, QueryTriggerInteraction.Ignore)
                    || !Physics.Raycast(rayHead, distance, _obstacle, QueryTriggerInteraction.Ignore))
                    hurtableObjects.Add(hurtableObject);
            }

            return hurtableObjects.ToArray();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (1 / _speed < _delay) _speed = 1 / _delay;
        }

        private void OnDrawGizmos() {
            if(_attackOrigin == null) return;
            
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(_attackOrigin.position, _attackOrigin.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _size);            
            
            Gizmos.color = Color.white;
        }
#endif
    }
}