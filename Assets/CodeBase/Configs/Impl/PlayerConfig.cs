using UnityEngine;

namespace CodeBase.Configs.Impl {
    [CreateAssetMenu(fileName = nameof(PlayerConfig), menuName = "Configs/" + nameof(PlayerConfig))]
    public class PlayerConfig : ScriptableObject, IPlayerConfig {
        [SerializeField][Min(0)] private float _staminaPrice = 1;
        [SerializeField][Min(0)] private float _recovery = 1;
        
        public float StaminaPrice => _staminaPrice;
        public float Recovery => _recovery;
    }
}