using System;

namespace CodeBase.Gameplay.General.Fighting.Ammunition.Impl {
    public class StandardAmmo : IAmmo {
        private int _current;
        public int Max { get; }

        public int Current {
            get => _current;
            set => _current = Math.Clamp(value, 0, Max);
        }

        public StandardAmmo(int max) {
            Max = max;
            Current = Max;
        }
    }
}