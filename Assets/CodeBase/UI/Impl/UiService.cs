using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.UI.Impl {
    public class UiService : IUiService, IDisposable {
        private readonly IAssetReferenceContainer _assetReferenceContainer;
        private readonly IAssetProvider _assetProvider;
        private readonly IInstantiateService _instantiateService;
        private readonly Canvas _uiRoot;
        private readonly CancellationToken _cancellationToken;
        private readonly Dictionary<Type, GameObject> _cachedScreens = new();
        
        private GameObject[] _screenPrefabs;

        public UiService(
            IAssetReferenceContainer assetReferenceContainer,
            IAssetProvider assetProvider,
            IInstantiateService instantiateService,
            Canvas uiRoot,
            CancellationToken cancellationToken
        ) {
            _assetReferenceContainer = assetReferenceContainer;
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _uiRoot = uiRoot;
            _cancellationToken = cancellationToken;
        }

        public async UniTask Initialize() {
            _screenPrefabs = await _assetProvider.LoadAssets<GameObject>(_assetReferenceContainer.Screens);
            if (_screenPrefabs.Length == 0) throw new ArgumentNullException();
            _assetProvider.Release(_assetReferenceContainer.Screens);
        }

        public async UniTask<TScreen> Open<TScreen>() where TScreen : IScreen {
            while (_screenPrefabs == null || _screenPrefabs.Length == 0) {
                await UniTask.Yield();
                if (_cancellationToken.IsCancellationRequested) return default;
            }

            if (!_cachedScreens.ContainsKey(typeof(TScreen))) {
                foreach (var prefab in _screenPrefabs) {
                    if(!prefab.TryGetComponent<TScreen>(out _)) continue;
                    _cachedScreens[typeof(TScreen)] = prefab;
                }

                if (!_cachedScreens.ContainsKey(typeof(TScreen)))
                    throw new ArgumentException($"There's no screen of Type = {typeof(TScreen)}");
            }
            
            var screen = _instantiateService.Instantiate(_cachedScreens[typeof(TScreen)], _uiRoot.transform);
            var screenComponent = screen.GetComponent<TScreen>();
            screenComponent.Open();
            return screenComponent;
        }

        public void Close<TScreen>(TScreen screen) where TScreen : IScreen {
            screen.Hide();
        }

        public void Dispose() {
            if(_screenPrefabs == null || _screenPrefabs.Length == 0) return;
            _assetProvider.Release(_assetReferenceContainer.Screens);
        }
    }
}