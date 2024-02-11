using CodeBase.Metaplay.Impl;
using Zenject;

namespace CodeBase.Architecture.Installers {
    public class MetaplayInstaller: MonoInstaller {
        public override void InstallBindings() {
            Container.BindService<PlayerMoneyBank>();
            Container.BindService<PlayerStatPointsBank>();
        }
    }
}