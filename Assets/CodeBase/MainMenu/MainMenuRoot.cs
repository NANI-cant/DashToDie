using System.Threading;
using CodeBase.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using Zenject;

namespace CodeBase.MainMenu {
    public class MainMenuRoot: IInitializable {
        private readonly IUiService _uiService;
        private readonly CancellationToken _cancel;

        public MainMenuRoot(
            IUiService uiService,
            CancellationToken cancel
        ) {
            _uiService = uiService;
            _cancel = cancel;
        }
        
        public void Initialize() { 
            InitAsync().Forget();
        }

        private async UniTaskVoid InitAsync() {
            await _uiService.Initialize();
            if(_cancel.IsCancellationRequested) return;

            await _uiService.Open<UI.Screens.MainMenu>();
        }
    }
}