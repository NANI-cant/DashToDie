using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.Player {
    public interface IPlayerFactory {
        UniTask<GameObject> Create();
    }
}