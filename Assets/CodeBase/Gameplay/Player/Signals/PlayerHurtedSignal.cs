using UnityEngine;

namespace CodeBase.Gameplay.Player.Signals {
    public class PlayerHurtedSignal {
        public GameObject PlayerObject { get; }

        public PlayerHurtedSignal(GameObject playerObject) {
            PlayerObject = playerObject;
        }
    }
}