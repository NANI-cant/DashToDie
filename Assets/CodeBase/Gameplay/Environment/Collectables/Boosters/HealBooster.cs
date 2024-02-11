using CodeBase.Gameplay.General;
using UnityEngine;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class HealBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(0)] private int _healingPoints;

        public void Apply(GameObject target) {
            var health = target.GetComponent<IHealth>();
            health.IncreaseHealth(_healingPoints);
        }
    }
}