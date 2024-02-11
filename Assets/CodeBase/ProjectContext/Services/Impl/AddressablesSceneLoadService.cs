using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace CodeBase.ProjectContext.Services.Impl {
    public class AddressablesSceneLoadService : ISceneLoadService {
        private readonly Dictionary<string, AsyncOperationHandle> _savedHandles = new();

        public AsyncOperationHandle LoadSceneAsync(AssetReference reference, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true) {
            var handle = Addressables.LoadSceneAsync(reference, mode, activateOnLoad);
            _savedHandles[reference.AssetGUID] = handle;
            return handle;
        }

        public void LoadSceneSync(AssetReference reference, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true) {
            var handle = Addressables.LoadSceneAsync(reference, mode, activateOnLoad);
            _savedHandles[reference.AssetGUID] = handle;
            handle.WaitForCompletion();
        }

        public void UnloadScene(AssetReference reference) {
            Addressables.UnloadSceneAsync(_savedHandles[reference.AssetGUID]);
            _savedHandles.Remove(reference.AssetGUID);
        }
    }
}