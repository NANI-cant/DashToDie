using System;
using UnityEngine;

namespace CodeBase.Gameplay.General.Impl {
    public class Health : MonoBehaviour, IHealth {
        [SerializeField][Min(1)] private int _maxHealth;
        [SerializeField][Min(0)] private int _healthPoints;

        public int MaxHealth => _maxHealth;
        public int HealthPoints => _healthPoints;
        
        public event Action Changed;

        private void Start() {
            _healthPoints = _maxHealth;
            Changed?.Invoke();
        }

        public void IncreaseHealth(int increasing) {
            if (increasing < 0) throw new ArgumentOutOfRangeException($"Increasing is {increasing}");

            _healthPoints += increasing;
            Changed?.Invoke();
        }

        public void DecreaseHealth(int decreasing) {
            if (decreasing < 0) throw new ArgumentOutOfRangeException($"Decreasing is {decreasing}");

            _healthPoints -= decreasing;
            _healthPoints = Mathf.Max(_healthPoints, 0);
            Changed?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (_healthPoints > _maxHealth) _healthPoints = _maxHealth;
        }
#endif
    }
}