using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.VFX;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    public class Slasher: MonoBehaviour, ICancelable {
        [Header("Stats")]
        [SerializeField] private ProgressiveFloat _staminaPrice;
        [SerializeField] private ProgressiveFloat _recovery;
        
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshTrail _trail;
        [SerializeField] private Collider _hurtBox;

        [Header("Setup")]
        [SerializeField][Min(0)] private float _speed = 100;
        [SerializeField] private Transform _slashOrigin;
        [SerializeField] private LayerMask _obstacles;
        [SerializeField] private LayerMask _enemies;

        private ITimeProvider _time;
        private IFixedTimeProvider _fixedTime;
        private DirectionMover _mover;
        private Stamina _stamina;
        private Transform _transform;
        private IDamageDealer _damageDealer;
        private Rigidbody _rigidbody;
        private CancellationTokenSource _executeTokenSource;
        private CancellationToken _destroyToken;

        public float ChargedDistance { get; private set; }
        
        public Transform SlashOrigin => _slashOrigin;
        public ProgressiveFloat StaminaPrice => _staminaPrice;
        public ProgressiveFloat Recovery => _recovery;

        [Inject]
        public void Construct(
            ITimeProvider time,
            IFixedTimeProvider fixedTime
        ) {
            _time = time;
            _fixedTime = fixedTime;
        }

        private void Awake() {
            _mover = GetComponent<DirectionMover>();
            _stamina = GetComponent<Stamina>();
            _transform = transform;
            
            _damageDealer = GetComponent<IDamageDealer>();
            _rigidbody = GetComponent<Rigidbody>();

            _trail.Disable();

            _destroyToken = this.GetCancellationTokenOnDestroy();
        }

        private void OnDestroy() => Cancel();

        public void Initialize(float staminaPrice, float recovery) {
            _staminaPrice = new ProgressiveFloat(staminaPrice);
            _recovery = new ProgressiveFloat(recovery);
        }

        public void ChargeTo(Vector3 point) {
            if(_executeTokenSource != null) return;
            if (ChargedDistance == 0) {
                _time.SlowDown(10);
                _mover.Disable();
            }
            
            _transform.forward = _transform.position.DirectionTo(point);
            ChargedDistance = Mathf.Min(
                Vector3.Distance(_transform.position, point),
                _stamina.Value / _staminaPrice.Value
            );
        }

        public void Execute() {
            if(ChargedDistance == 0) return;
            if(_executeTokenSource != null) return;
            
            _executeTokenSource = new CancellationTokenSource();
            ExecuteAsync(_executeTokenSource.Token).Forget();
        }

        public void Cancel() {
            Execute();

            _executeTokenSource?.Cancel();
            _executeTokenSource?.Dispose(); 
            _executeTokenSource = null;
        }

        private async UniTask ExecuteAsync(CancellationToken cancelToken) {
            _animator.SetTrigger("Slash");
            _trail.Enable();
            _time.SpeedUp(10);
            _hurtBox.enabled = false;
            
            await UniTask.NextFrame();
            if(_destroyToken.IsCancellationRequested) return;

            await SlashAsync(cancelToken);
            if(_destroyToken.IsCancellationRequested) return;
            ChargedDistance = 0; 
            _hurtBox.enabled = true;
            
            await RecoverAsync(cancelToken);
            if(_destroyToken.IsCancellationRequested) return;
            _trail.Disable();
            _mover.Enable();

            _executeTokenSource = null;
        }

        private async Task SlashAsync(CancellationToken cancelToken) {
            float slashDistance = ChargedDistance;
            if (Physics.Raycast(SlashOrigin.position, SlashOrigin.forward, out var raycastHit, slashDistance, _obstacles)) {
                slashDistance = raycastHit.distance;
            }
            _stamina.Spend(slashDistance * _staminaPrice.Value);
            
            List<IHurtable> hurtOnSlashCache = new List<IHurtable>();
            while (slashDistance > 0 && !cancelToken.IsCancellationRequested && !_destroyToken.IsCancellationRequested) {
                await UniTask.WaitForFixedUpdate();
                float translationDistance = _speed * _fixedTime.FixedDelaTime;
                translationDistance = Mathf.Min(translationDistance, slashDistance);

                Vector3 lastPosition = _rigidbody.position;
                _rigidbody.MovePosition(lastPosition + _transform.forward * translationDistance);
                Vector3 newPosition = _rigidbody.position;

                HitOnPath(lastPosition, newPosition, hurtOnSlashCache);

                slashDistance -= translationDistance;
            }
        }

        private async UniTask RecoverAsync(CancellationToken cancelToken) {
            float recovering = Recovery.Value;

            while (recovering > 0 && !cancelToken.IsCancellationRequested && !_destroyToken.IsCancellationRequested) {
                await UniTask.NextFrame();
                recovering -= _time.DelaTime;
                recovering = Mathf.Max(0, recovering);
            }
        }

        private void HitOnPath(Vector3 from, Vector3 to, ICollection<IHurtable> hurtOnSlashCache) {
            Vector3 center = (from + to) / 2 + Vector3.up * 0.5f;
            Vector3 halfExtends = new Vector3(Mathf.Abs(to.x - from.x) / 2, 0.5f, Mathf.Abs(to.z - from.z) / 2);
            Collider[] collidersOnPath = Physics.OverlapBox(center, halfExtends, Quaternion.identity, _enemies, QueryTriggerInteraction.Collide);

            foreach (var hurtableCollider in collidersOnPath) {
                if(!hurtableCollider.TryGetComponent<IHurtable>(out var hurtable)) continue;
                if(hurtOnSlashCache.Contains(hurtable)) continue;
                
                hurtable.TakeHit(_damageDealer.Damage.Value, out _);
                hurtOnSlashCache.Add(hurtable);
            }
        }
    }
}