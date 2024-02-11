using System;

namespace CodeBase.Metaplay.Impl {
    public class PlayerStatPointsBank : IStatPointsBank {
        public int All { get; private set; }
        public int Available { get; private set; }
        
        public event Action Changed;
        
        public bool CanTake(int count) {
            if (count < 0) throw new ArgumentOutOfRangeException();
            return count <= Available;
        }

        public void Earn(int count) {
            if (count < 0) throw new ArgumentOutOfRangeException();
            
            Available += count;
            All += count;
            Changed?.Invoke();
        }

        public void Put(int count) {
            if (count < 0) throw new ArgumentOutOfRangeException();
            if (All < Available + count) throw new ArgumentOutOfRangeException();
            
            Available += count;
            Changed?.Invoke();
        }

        public void Take(int count) {
            if (count < 0) throw new ArgumentOutOfRangeException();
            if (!CanTake(count)) throw new ArgumentOutOfRangeException();
            
            Available -= count;
            Changed?.Invoke();
        }
    }
}