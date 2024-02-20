using CodeBase.Gameplay.Enemies.AI;
using CodeBase.Gameplay.General.Fighting.Ammunition;
using CodeBase.Gameplay.General.Fighting.Ammunition.Impl;

namespace CodeBase.Gameplay.General.Fighting {
    public class PistolAttacker : BulletAttacker {
        protected override IAmmo Init(int ammoCount) => new InfinityAmmo();
    }
}