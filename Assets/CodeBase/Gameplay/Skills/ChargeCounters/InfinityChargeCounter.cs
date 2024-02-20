﻿using System;

namespace CodeBase.Gameplay.Skills.ChargeCounters {
    public class InfinityChargeCounter : IChargeCounter {
        public event Action Changed;

        public int Charges {
            get => 1;
            set { }
        }
    }
}