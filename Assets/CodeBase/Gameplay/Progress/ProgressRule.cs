using UnityEngine;

namespace CodeBase.Gameplay.Progress {
    public static class ProgressRule {
        public static float Linear(float baseValue, float multiplier, float level) {
            return baseValue + level * multiplier;
        }
        
        public static float Degree(float baseValue, float multiplier, float level) {
            return baseValue + Mathf.Pow(level, multiplier);
        }
    }
}