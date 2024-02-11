using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace CodeBase.ProjectContext.Services {
    public interface IAssetProvider {
        UniTask Initialize();
        UniTask<TAsset> LoadAsset<TAsset>(AssetReference reference) where TAsset : class;
        UniTask<TAsset[]> LoadAssets<TAsset>(AssetReference[] references) where TAsset : class;
        void Release(AssetReference reference);
        void Release(AssetReference[] references);
    }
}