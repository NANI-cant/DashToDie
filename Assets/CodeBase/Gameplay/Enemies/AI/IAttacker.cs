using UnityEngine;

namespace CodeBase.Gameplay.Enemies.AI {
    public interface IAttacker {
        bool IsCharged { get; }
        
        void Attack();
        bool CheckTargetInRange(Transform target);
        void Cancel();
    }
}