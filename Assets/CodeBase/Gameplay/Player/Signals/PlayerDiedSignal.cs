using UnityEngine;

namespace CodeBase.Gameplay.Player.Signals {
    public class PlayerDiedSignal {
        public GameObject PlayerObject { get; }

        public PlayerDiedSignal(GameObject playerObject) {
            PlayerObject = playerObject;
        }
    }
}