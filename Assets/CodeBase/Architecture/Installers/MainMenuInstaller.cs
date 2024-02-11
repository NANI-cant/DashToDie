using System.Threading;
using CodeBase.MainMenu;
using CodeBase.UI.Impl;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Architecture.Installers {
    public class MainMenuInstaller: MonoInstaller {
        [SerializeField] private Canvas _uiRoot;
        
        public override void InstallBindings() {
            Container.BindService<UiService>();
            Container.BindService<MainMenuRoot>();

            Container
                .BindInstance(_uiRoot)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<CancellationToken>()
                .FromInstance(this.GetCancellationTokenOnDestroy())
                .AsSingle()
                .NonLazy();
        }
    }
}