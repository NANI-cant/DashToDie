using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Configs.Impl {
    [CreateAssetMenu(fileName = nameof(ProgressConfig), menuName = "Configs/"+nameof(ProgressConfig))]
    public class ProgressConfig : ScriptableObject, IProgressConfig {
        [Header("Money")]
        [SerializeField][Min(0)] private int _baseMoney;
        [SerializeField][Min(0)] private float _moneyMultiplier;
        
        [Header("Skill Points")]
        [SerializeField][Min(0)] private float _skillPointMultiplier;
        [SerializeField][Range(0, 1)] private float _skillPointMaxReducing = 0.5f;
        [SerializeField][Min(0)] private int _maxSkillPoints;

        public int GetMoneyForKill(int level) {
            return (int) (_baseMoney + _moneyMultiplier * level * _baseMoney);
        }

        public int GetSkillPointsForRun(int levelsPassed, int allSkillPoints) {
            float reducingRatio = Mathf.InverseLerp(0, _maxSkillPoints, allSkillPoints);
            float reducing = _skillPointMaxReducing * reducingRatio;

            int pointsToAdd = (int) (Mathf.Exp(_skillPointMultiplier * (levelsPassed - 1)) * (1 - reducing));
            pointsToAdd = Mathf.Clamp(pointsToAdd, 0, _maxSkillPoints - allSkillPoints);
            return pointsToAdd;
        }
    }
}