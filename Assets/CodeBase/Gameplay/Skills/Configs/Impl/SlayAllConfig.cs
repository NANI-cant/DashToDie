using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Gameplay.Skills.Configs.Impl {
    [CreateAssetMenu(fileName = nameof(SlayAllConfig), menuName = "Skills/"+nameof(SlayAllConfig))]
    public class SlayAllConfig : ASkillConfig {
        [SerializeField] private float _radius;
        [SerializeField] private float _delay;
        [SerializeField] private AssetReferenceGameObject _vfxReference;
        

        public float Radius => _radius;
        public float Delay => _delay;
        public AssetReferenceGameObject VFXReference => _vfxReference;
    }
}