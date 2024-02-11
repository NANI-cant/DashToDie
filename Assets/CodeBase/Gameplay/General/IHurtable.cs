namespace CodeBase.Gameplay.General {
    public interface IHurtable {
        void TakeHit(int damage, out bool isStillAlive);
    }
}