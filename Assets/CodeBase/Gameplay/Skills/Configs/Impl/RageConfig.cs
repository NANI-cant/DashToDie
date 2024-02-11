using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Gameplay.Skills.Configs.Impl {
    [CreateAssetMenu(fileName = nameof(RageConfig), menuName = "Skills/"+nameof(RageConfig))]
    public class RageConfig : ASkillConfig {
        [SerializeField] private float _duration;
        [SerializeField] private float _multiplier;
        [SerializeField] private AssetReference _vfxReference;

        public float Duration => _duration;
        public float Multiplier => _multiplier;
        public AssetReference VFXReference => _vfxReference;
    }
}