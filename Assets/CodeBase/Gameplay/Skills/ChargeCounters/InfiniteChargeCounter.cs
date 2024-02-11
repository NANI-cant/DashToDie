using System;

namespace CodeBase.Gameplay.Skills.ChargeCounters {
    public class InfiniteChargeCounter : IChargeCounter {
        public event Action Changed;

        public int Charges {
            get => 1;
            set { }
        }
    }
}