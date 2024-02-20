using System;
using System.Threading;
using CodeBase.Gameplay.General.Impl;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.Skills.ChargeCounters;
using CodeBase.Gameplay.VFX;
using Cysharp.Threading.Tasks;

namespace CodeBase.Gameplay.Skills.Impl {
    public class HyperArmorSkill : ASkillBase {
        private readonly HyperArmorVfx _vfx;
        public override IChargeCounter ChargeCounter { get; } = new StandardChargeCounter();
        
        public ProgressiveFloat Duration { get; }

        public HyperArmorSkill(float duration, HyperArmorVfx vfx) {
            _vfx = vfx;
            Duration = new ProgressiveFloat(duration);
        }

        protected override async UniTask ExecuteSkillAsync(ASkillHolder holder, int holderTeam, CancellationToken cancelToken) {
            var hurtProcessor = holder.GetComponent<HurtProcessor>();
            hurtProcessor._resistance += 1;
            if (_vfx != null) _vfx.Enable();
            try {
                await UniTask.Delay((int) (Duration.Value * 1000), cancellationToken: cancelToken);
            }
            catch (OperationCanceledException) { }
            finally {
                if (_vfx != null) _vfx.Disable();   
                if (hurtProcessor != null) {
                    hurtProcessor._resistance -= 1;    
                }
            }
        }
    }
}