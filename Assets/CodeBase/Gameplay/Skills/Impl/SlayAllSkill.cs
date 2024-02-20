using System;
using System.Threading;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.General.Brains;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.Gameplay.Skills.ChargeCounters;
using CodeBase.Gameplay.VFX;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.Skills.Impl {
    public class SlayAllSkill : ASkillBase{
        private readonly GameObject _vfxPrefab;
        private readonly IInstantiateService _instantiateService;

        public override IChargeCounter ChargeCounter { get; } = new StandardChargeCounter();

        public ProgressiveFloat Radius { get; }
        public ProgressiveFloat Delay { get; }

        public SlayAllSkill(
            float radius,
            float delay,
            GameObject vfxPrefab,
            IInstantiateService instantiateService
        ) {
            Radius = new ProgressiveFloat(radius);
            Delay = new ProgressiveFloat(delay);
            _vfxPrefab = vfxPrefab;
            _instantiateService = instantiateService;
        }

        protected override async UniTask ExecuteSkillAsync(ASkillHolder holder, int holderTeam, CancellationToken cancelToken) {
            var vfx = _instantiateService.Instantiate(_vfxPrefab).GetComponent<SlayAllVfx>();
            vfx.transform.position = holder.transform.position;

            try {
                holder.TryGetComponent<IBrain>(out var brain);
                brain?.Disable();

                vfx.Run(Delay.Value, Radius.Value);
                await UniTask.Delay((int) (Delay.Value * 1000), cancellationToken: cancelToken);

                HitEnemies(holder, holderTeam);
            }
            catch (OperationCanceledException) {
                vfx.Cancel();
            }
            finally {
                if (holder != null) {
                    holder.TryGetComponent<IBrain>(out var brain);
                    brain?.Enable();
                }
            }
        }

        private void HitEnemies(ASkillHolder holder, int holderTeam) {
            var collidersInRange = Physics.OverlapSphere(holder.transform.position, Radius.Value);
            foreach (var collider in collidersInRange) {
                if (!collider.TryGetComponent<IHurtable>(out var hurtable)) continue;
                if (collider.TryGetComponent<ITeamProvider>(out var teamProvider) &&
                    teamProvider.TeamId == holderTeam) continue;

                hurtable.TakeHit(holder.GetComponent<IDamageDealer>().Damage.Value, out _);
            }
        }
    }
}