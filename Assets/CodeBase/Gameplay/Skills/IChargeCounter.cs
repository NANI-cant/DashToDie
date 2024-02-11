using System;

namespace CodeBase.Gameplay.Skills {
    public interface IChargeCounter {
        event Action Changed;
        int Charges { get; set; }
    }
}