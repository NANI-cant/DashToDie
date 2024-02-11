using UnityEngine;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public interface IBooster {
        void Apply(GameObject target);
    }
}