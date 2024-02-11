using System;

namespace CodeBase.Metaplay.Impl {
    public class PlayerMoneyBank : IMoneyBank {
        public int Amount { get; private set; }
        
        public event Action Changed;
        public event Action Earned;
        public event Action Spent;
        
        public bool CanSpend(int value) => Amount >= value;

        public void Earn(int value) {
            if (value < 0) throw new ArgumentOutOfRangeException($"Earn value must be >= 0, now {value}");
            
            Amount += value;
            
            Changed?.Invoke();
            Earned?.Invoke();
        }

        public bool TrySpend(int value) {
            if (value < 0) throw new ArgumentOutOfRangeException($"Spend value must be >= 0, now {value}");
            if (!CanSpend(value)) return false;

            Amount -= value;
            
            Changed?.Invoke();
            Spent?.Invoke();
            return true;
        }
    }
}