using System;
using System.Threading;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.Skills.ChargeCounters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.Skills.Impl {
    public class RageSkill: ASkillBase {
        private readonly GameObject _vfx;

        public override IChargeCounter ChargeCounter { get; } = new StandardChargeCounter();

        public ProgressiveFloat Duration { get; }
        public ProgressiveFloat Multiplier { get; }

        public RageSkill(float duration, float multiplier, GameObject vfx) {
            Duration = new ProgressiveFloat(duration);
            Multiplier = new ProgressiveFloat(multiplier);
            _vfx = vfx;
            
            _vfx.Deactivate();
        }

        protected override async UniTask ExecuteSkillAsync(ASkillHolder holder, int holderTeam, CancellationToken cancelToken) {
            var slashCharger = holder.GetComponent<SlashCharger>();
            slashCharger.StaminaPrice.Decrease(Multiplier.Value);
            try {
                _vfx.Activate();
                await UniTask.Delay((int) (Duration.Value * 1000), cancellationToken: cancelToken);
            }
            catch (OperationCanceledException) { }
            finally {
                if (slashCharger != null) {
                    slashCharger.StaminaPrice.Increase(Multiplier.Value);
                    _vfx.Deactivate();   
                }
            }
        }
    }
}