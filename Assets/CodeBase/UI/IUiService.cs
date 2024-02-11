using Cysharp.Threading.Tasks;

namespace CodeBase.UI {
    public interface IUiService {
        UniTask Initialize();
        UniTask<TScreen> Open<TScreen>() where TScreen : IScreen;
        void Close<TScreen>(TScreen screen) where TScreen : IScreen;
    }
}