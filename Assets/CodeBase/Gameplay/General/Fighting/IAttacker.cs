using CodeBase.Gameplay.General.Fighting.Ammunition;
using JetBrains.Annotations;

namespace CodeBase.Gameplay.General.Fighting {
    public interface IAttacker: ICancelable {
        [CanBeNull] IAmmo Ammo { get; }
        bool IsCharging { get; }
        bool IsReloading { get; }
        
        void Attack();
        void Reload();
    }
}