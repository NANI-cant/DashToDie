using System;
using UnityEngine.Serialization;

namespace CodeBase.Gameplay.General.Impl {
    public class ShieldModifier : IHurtModifier {
        public event Action Removed;
        
        public int ProcessDamage(int damage, HurtProcessor processor) {
            if (damage == 0) return damage;
            
            processor.RemoveModifier(this);
            Removed?.Invoke();
            return 0;
        }
    }
}