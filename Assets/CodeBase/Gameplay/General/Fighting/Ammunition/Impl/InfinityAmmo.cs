namespace CodeBase.Gameplay.General.Fighting.Ammunition.Impl {
    public class InfinityAmmo : IAmmo {
        public int Max => 1;
        public int Current { 
            get => 1;
            set { }
        }
    }
}