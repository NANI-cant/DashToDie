using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Screens {
    public class Hud: MonoBehaviour, IScreen {
        private IDestroyProvider _destroyProvider;

        [Inject]
        public void Construct(IDestroyProvider destroyProvider) {
            _destroyProvider = destroyProvider;
        }
        
        public void Open() {
            gameObject.Activate();
        }

        public void Hide() {
            _destroyProvider.Destroy(gameObject);
        }
    }
}