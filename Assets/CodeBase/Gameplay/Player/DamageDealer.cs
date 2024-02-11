using CodeBase.Gameplay.General;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.Skills;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    public class DamageDealer : MonoBehaviour, IDamageDealer {
        [SerializeField] private ProgressiveInt _damage = new(1);

        public ProgressiveInt Damage => _damage;
    }
}