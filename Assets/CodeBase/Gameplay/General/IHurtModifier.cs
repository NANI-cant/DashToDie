using CodeBase.Gameplay.General.Impl;

namespace CodeBase.Gameplay.General {
    public interface IHurtModifier {
        int ProcessDamage(int damage, HurtProcessor processor);
    }
}