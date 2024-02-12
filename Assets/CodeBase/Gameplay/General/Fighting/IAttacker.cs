using UnityEngine;

namespace CodeBase.Gameplay.General.Fighting {
    public interface IAttacker {
        bool IsCharged { get; }
        
        void Attack();
        bool CheckTargetInRange(Transform target);
        void Cancel();
    }
}