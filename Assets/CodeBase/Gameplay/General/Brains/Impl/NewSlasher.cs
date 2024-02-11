using System.Collections.Generic;
using System.Threading;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.VFX;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.General.Brains.Impl {
    public class NewSlasher: MonoBehaviour, ICancelable {
        [SerializeField] private ProgressiveFloat _staminaPrice;
        [SerializeField] private ProgressiveFloat _recovery;
        
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshTrail _trail;
        
        [SerializeField] private Collider _hurtBox;
        [SerializeField][Min(0)] private float _speed = 100;
        [SerializeField] private Transform _slashOrigin;
        [SerializeField] private LayerMask _obstacles;
        [SerializeField] private LayerMask _enemies;
        
        private CancellationTokenSource _chargeTokenSource;
        private CancellationTokenSource _executeTokenSource;
        private CancellationTokenSource _recoverTokenSource;

        private readonly List<IHurtable> _hurtOnSlashCache = new();
        
        private ITimeProvider _time;
        private IInputService _input;
        private IFixedTimeProvider _fixedTime;
        private DirectionMover _mover;
        private Stamina _stamina;
        private Transform _transform;
        private float _chargedDistance;
        private IDamageDealer _damageDealer;
        private Rigidbody _rigidbody;

        public Transform SlashOrigin => _slashOrigin;
        public float ChargedDistance => _chargedDistance;
        public ProgressiveFloat StaminaPrice => _staminaPrice;
        public ProgressiveFloat Recovery => _recovery;

        [Inject]
        public void Construct(
            ITimeProvider timeProvider, 
            IInputService input,
            IFixedTimeProvider fixedTime
        ) {
            _time = timeProvider;
            _input = input;
            _fixedTime = fixedTime;
        }

        private void Awake() {
            _mover = GetComponent<DirectionMover>();
            _stamina = GetComponent<Stamina>();
            _transform = transform;
            
            _damageDealer = GetComponent<IDamageDealer>();
            _rigidbody = GetComponent<Rigidbody>();

            _trail.Disable();
        }
        
        public void Initialize(float staminaPrice, float recovery) {
            _staminaPrice = new ProgressiveFloat(staminaPrice);
            _recovery = new ProgressiveFloat(recovery);
        }

        public void Charge() {
            if(!_chargeTokenSource.IsCancellationRequested) return;
            _chargeTokenSource = new CancellationTokenSource();
            ChargeAsync(_chargeTokenSource.Token).Forget();
        }

        public void Execute() {
            if(!_executeTokenSource.IsCancellationRequested) return;
            
            _chargeTokenSource.Cancel();
            _executeTokenSource = new CancellationTokenSource();

            ExecuteAsync(_executeTokenSource.Token).Forget();
        }

        private async UniTask ChargeAsync(CancellationToken cancelToken) {
            _chargedDistance = 0;
            _time.SlowDown(10);
            _mover.Disable();

            while (!cancelToken.IsCancellationRequested) {
                Ray pointerRay = _input.GetPointerRay(Camera.main);

                Plane characterPlane = _transform.GetPlane();
                if (!characterPlane.Raycast(pointerRay, out float rayLength)) {
                    var inversePointerRay = new Ray(pointerRay.origin, -pointerRay.direction);
                    characterPlane.Raycast(inversePointerRay, out rayLength);
                }

                Vector3 destination = pointerRay.GetPoint(rayLength);

                _transform.forward = _transform.position.DirectionTo(destination);
                _chargedDistance = Mathf.Min(
                    Vector3.Distance(_transform.position, destination),
                    _stamina.Value / _staminaPrice.Value
                );
                
                await UniTask.NextFrame();
            }
            
            _time.SpeedUp(10);
        }

        private async UniTask ExecuteAsync(CancellationToken cancelToken) {
            _animator.SetTrigger("Slash");
            _trail.Enable();
            _hurtBox.enabled = false;

            float slashDistance = _chargedDistance;
            if (Physics.Raycast(SlashOrigin.position, SlashOrigin.forward, out var raycastHit, slashDistance, _obstacles)) {
                slashDistance = raycastHit.distance;
            }

            _stamina.Spend(slashDistance * _staminaPrice.Value);
            _hurtOnSlashCache.Clear();

            while (slashDistance > 0 && !cancelToken.IsCancellationRequested) {
                await UniTask.WaitForFixedUpdate();
                float translationDistance = _speed * _fixedTime.FixedDelaTime;
                translationDistance = Mathf.Min(translationDistance, slashDistance);
            
                Vector3 lastPosition = _rigidbody.position;
                _rigidbody.MovePosition(lastPosition + _transform.forward * translationDistance);
                Vector3 newPosition = _rigidbody.position;

                HitOnPath(lastPosition, newPosition);

                slashDistance -= translationDistance;
            }
            
            _trail.Disable();
            _hurtBox.enabled = true;

            _recoverTokenSource.Cancel();
            _recoverTokenSource = new CancellationTokenSource();
            RecoverAsync(_recoverTokenSource.Token);
        }

        private async UniTask RecoverAsync(CancellationToken cancelToken) {
            float recovering = Recovery.Value;

            while (recovering > 0 && !cancelToken.IsCancellationRequested) {
                recovering -= _time.DelaTime;
                recovering = Mathf.Max(0, recovering);
            }

            _mover.Enable();
        }

        public void Cancel() {
            _chargeTokenSource?.Cancel();
            _executeTokenSource?.Cancel();
            _recoverTokenSource?.Cancel();
        }
        
        private void HitOnPath(Vector3 from, Vector3 to) {
            Vector3 center = (from + to) / 2 + Vector3.up * 0.5f;
            Vector3 halfExtends = new Vector3(Mathf.Abs(to.x - from.x) / 2, 0.5f, Mathf.Abs(to.z - from.z) / 2);
            Collider[] collidersOnPath = Physics.OverlapBox(center, halfExtends, Quaternion.identity, _enemies, QueryTriggerInteraction.Collide);

            foreach (var hurtableCollider in collidersOnPath) {
                if(!hurtableCollider.TryGetComponent<IHurtable>(out var hurtable)) continue;
                if(_hurtOnSlashCache.Contains(hurtable)) continue;
                
                hurtable.TakeHit(_damageDealer.Damage.Value, out _);
                _hurtOnSlashCache.Add(hurtable);
            }
        }
    }
}