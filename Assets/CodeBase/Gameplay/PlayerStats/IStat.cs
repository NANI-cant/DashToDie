using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats {
    public interface IStat {
        void Apply(GameObject playerObject);
    }
}