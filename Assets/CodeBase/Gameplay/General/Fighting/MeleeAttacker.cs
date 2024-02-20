using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Gameplay.General.Cooldowning.Impl;
using CodeBase.Gameplay.General.Fighting.Ammunition;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace CodeBase.Gameplay.General.Fighting {
    public class MeleeAttacker: MonoBehaviour, IAttacker {
        [SerializeField][Min(0.0000001f)] private float _speed;
        [SerializeField][Min(0)] private int _damage;
        [SerializeField][Min(0.0000001f)] private float _delay = 0.1f;
        [SerializeField] private Vector3 _size;
        [SerializeField] private LayerMask _enemy;

        [SerializeField] private Transform _attackOrigin;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnityEvent _onHit;

        private ITimeProvider _timeProvider;
        private CancellationTokenSource _attackProcess;

        private Cooldown _cooldown = new(0);
        private static readonly int AttackKey = Animator.StringToHash("Attack");

        public IAmmo Ammo => null;
        public bool IsCharging => _attackProcess != null;
        public bool IsReloading => false;

        [Inject]
        public void Construct(ITimeProvider timeProvider) {
            _timeProvider = timeProvider;
        }
        
        private void OnDestroy() => Cancel();
        private void Update() => _cooldown?.Tick(_timeProvider.DelaTime);

        public void Attack() {
            if(!_cooldown.IsTimesUp || IsCharging) return;

            _attackProcess = new CancellationTokenSource();
            ExecuteAttack(_attackProcess.Token).Forget();
        }

        public void Reload() { }

        public void Cancel() {
            _attackProcess?.Cancel();
            _attackProcess?.Dispose();
            _attackProcess = null;
        }

        private async UniTaskVoid ExecuteAttack(CancellationToken cancelToken) {
            try {
                _animator.SetTrigger(AttackKey);
                await UniTask.Delay((int) (_delay * 1000f), cancellationToken: cancelToken);

                IHurtable[] targets = FindHurtablesInRange();
                _onHit?.Invoke();
                DealHit(targets);
                _cooldown = new Cooldown(1 / _speed);
                _cooldown.WindUp();

                _attackProcess?.Dispose();
                _attackProcess = null;
            }
            catch (OperationCanceledException) { }
        }

        private IHurtable[] FindHurtablesInRange() {
            var enemies = Physics.OverlapBox(_attackOrigin.position, _size / 2, _attackOrigin.rotation, _enemy, QueryTriggerInteraction.Collide);

            List<IHurtable> hurtables = new List<IHurtable>();
            foreach (var possibleHurtable in enemies) {
                if(!possibleHurtable.TryGetComponent<IHurtable>(out var hurtable)) continue;
                hurtables.Add(hurtable);
            }

            return hurtables.ToArray();
        }

        private void DealHit(IHurtable[] targets) {
            foreach (var target in targets)
                target.TakeHit(_damage, out _);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (1 / _speed < _delay) _speed = 1 / _delay;
        }

        private void OnDrawGizmos() {
            if(_attackOrigin == null) return;
            
            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(_attackOrigin.position, _attackOrigin.rotation, Vector3.one);
            if (_attackProcess == null) {
                Gizmos.DrawWireCube(Vector3.zero, _size);    
            }
            else {
                Gizmos.DrawCube(Vector3.zero, _size);
            }
                        
            
            Gizmos.color = Color.white;
        }
#endif
    }
}