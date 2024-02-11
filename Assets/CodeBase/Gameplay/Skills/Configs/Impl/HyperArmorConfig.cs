using UnityEngine;

namespace CodeBase.Gameplay.Skills.Configs.Impl {
    [CreateAssetMenu(fileName = nameof(HyperArmorConfig), menuName = "Skills/"+nameof(HyperArmorConfig))]
    public class HyperArmorConfig: ASkillConfig {
        [SerializeField][Min(0)] private float _duration;

        public float Duration => _duration;
    }
}