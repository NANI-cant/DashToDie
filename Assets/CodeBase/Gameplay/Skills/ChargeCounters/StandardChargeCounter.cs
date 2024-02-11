using System;
using UnityEngine;

namespace CodeBase.Gameplay.Skills.ChargeCounters {
    public class StandardChargeCounter : IChargeCounter {
        private int _charges = 10;

        public event Action Changed;

        public int Charges {
            get => _charges;
            set {
                _charges = Mathf.Max(0, value);
                Changed?.Invoke();
            }
        }
    }
}