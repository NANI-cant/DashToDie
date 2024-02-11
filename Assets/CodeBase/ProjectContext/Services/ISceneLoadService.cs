using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace CodeBase.ProjectContext.Services {
    public interface ISceneLoadService {
        AsyncOperationHandle LoadSceneAsync(AssetReference reference, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true);
        void LoadSceneSync(AssetReference reference, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true);
        
        void UnloadScene(AssetReference reference);
    }
}