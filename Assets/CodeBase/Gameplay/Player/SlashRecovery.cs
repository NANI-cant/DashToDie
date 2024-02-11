using System;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(DirectionMover))]
    public class SlashRecovery: MonoBehaviour {
        [SerializeField] private ProgressiveFloat _recovery;
        
        private ITimeProvider _time;
        private DirectionMover _mover;

        public float Recovering { get; private set; }

        public ProgressiveFloat Recovery => _recovery;

        [Inject]
        public void Construct(ITimeProvider timeProvider) {
            _time = timeProvider;
        }

        private void Awake() {
            _mover = GetComponent<DirectionMover>();
        }

        public void Initialize(float recovery) {
            _recovery = new ProgressiveFloat(recovery);
        }

        private void OnEnable() {
            Recovering = Recovery.Value;
        }

        private void OnDisable() {
            _mover.Enable();
        }

        private void Update() {
            Recovering -= _time.DelaTime;
            Recovering = Mathf.Max(0, Recovering);
            
            if (Recovering <= 0) this.Disable();
        }
    }
}