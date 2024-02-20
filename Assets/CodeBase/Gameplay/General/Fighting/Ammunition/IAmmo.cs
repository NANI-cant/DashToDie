namespace CodeBase.Gameplay.General.Fighting.Ammunition {
    public interface IAmmo {
        int Max { get; }
        int Current { get; set; }
    }
}