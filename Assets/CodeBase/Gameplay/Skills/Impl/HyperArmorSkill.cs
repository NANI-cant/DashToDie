using System;
using System.Threading;
using CodeBase.Gameplay.General.Impl;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.Skills.ChargeCounters;
using Cysharp.Threading.Tasks;

namespace CodeBase.Gameplay.Skills.Impl {
    public class HyperArmorSkill : ASkillBase {
        public override IChargeCounter ChargeCounter { get; } = new StandardChargeCounter();
        
        public ProgressiveFloat Duration { get; }

        public HyperArmorSkill(float duration) {
            Duration = new ProgressiveFloat(duration);
        }

        protected override async UniTask ExecuteSkillAsync(ASkillHolder holder, int holderTeam, CancellationToken cancelToken) {
            var hurtProcessor = holder.GetComponent<HurtProcessor>();
            hurtProcessor.resistance += 1;
            try {
                await UniTask.Delay((int) (Duration.Value * 1000), cancellationToken: cancelToken);
            }
            catch (OperationCanceledException) { }
            finally {
                if (hurtProcessor != null) {
                    hurtProcessor.resistance -= 1;    
                }
            }
        }
    }
}