using System;
using UnityEngine;
using UnityEngine.Localization;

namespace CodeBase.Gameplay.PlayerStats {
    public abstract class AStatObject : ScriptableObject, IStat {
        [SerializeField] [Min(0)] private float _multiplierPerLevel;
        [SerializeField] [Min(0)] private int _level;
        [SerializeField] [Min(0)] private int _maxLevel = 100;
        [SerializeField] private LocalizedString _localizedString;
        
        public int Level {
            get => _level;
            set {
                if (value < 0) throw new ArgumentOutOfRangeException();
                if (value > _maxLevel) throw new ArgumentOutOfRangeException();
                _level = value;
            }
        }

        public int MaxLevel => _maxLevel;
        public float MultiplierPerLevel => _multiplierPerLevel;
        public float ResultMultiplier => Level * MultiplierPerLevel;
        public LocalizedString LocalizedString => _localizedString;

        public abstract void Apply(GameObject playerObject);
    }
}