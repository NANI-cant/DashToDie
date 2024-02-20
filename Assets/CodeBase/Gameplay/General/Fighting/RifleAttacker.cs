using CodeBase.Gameplay.Enemies.AI;
using CodeBase.Gameplay.General.Fighting.Ammunition;
using CodeBase.Gameplay.General.Fighting.Ammunition.Impl;

namespace CodeBase.Gameplay.General.Fighting {
    public class RifleAttacker : BulletAttacker {
        protected override IAmmo Init(int ammoCount) => new StandardAmmo(ammoCount);
    }
}