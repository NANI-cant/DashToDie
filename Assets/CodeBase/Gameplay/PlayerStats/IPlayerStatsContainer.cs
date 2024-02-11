using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats {
    public interface IPlayerStatsContainer {
        UniTask SetupPlayer(GameObject player);
    }
}