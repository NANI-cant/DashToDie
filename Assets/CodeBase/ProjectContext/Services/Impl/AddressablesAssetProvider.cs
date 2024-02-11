using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.ProjectContext.Services.Impl {
    public class AddressablesAssetProvider : IAssetProvider {
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        public async UniTask Initialize() {
            await Addressables.InitializeAsync().ToUniTask();
        }

        public async UniTask<TAsset> LoadAsset<TAsset>(AssetReference reference) where TAsset : class{
            AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(reference);
            AddHandle(reference.AssetGUID, handle);

            TAsset asset = await handle.ToUniTask();
            return asset;
        }

        public async UniTask<TAsset[]> LoadAssets<TAsset>(AssetReference[] references) where TAsset : class {
            TAsset[] assets = new TAsset[references.Length];
            List<UniTask<TAsset>> tasks = new List<UniTask<TAsset>>();
            foreach (var reference in references) {
                tasks.Add(LoadAsset<TAsset>(reference));
            }

            for (int i = 0; i < assets.Length; i++) {
                assets[i] = await tasks[i];
            }

            return assets;
        }

        public void Release(AssetReference[] references) {
            foreach (var reference in references) {
                Release(reference);
            }
        }

        public void Release(AssetReference reference) {
            string key = reference.AssetGUID;
            if (_handles.TryGetValue(key, out var assetHandles)) {
                Addressables.Release(assetHandles[0]);
                assetHandles.RemoveAt(0);
                if (assetHandles.Count == 0)
                    _handles.Remove(key);
            }
        }

        private void AddHandle<TAsset>(string key, AsyncOperationHandle<TAsset> handle) where TAsset : class {
            if (!_handles.TryGetValue(key, out var assetHandles)) {
                assetHandles = new List<AsyncOperationHandle>();
                _handles[key] = assetHandles;
            }

            assetHandles.Add(handle);
        }
    }
}