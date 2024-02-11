using UnityEngine;

namespace CodeBase.Gameplay.Environment.Collectables {
    public interface ICollectable {
        void Collect(GameObject collector);
    }
}