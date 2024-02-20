using System;
using System.Threading;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.Skills.Configs;
using CodeBase.Gameplay.Skills.Configs.Impl;
using CodeBase.Gameplay.Skills.Impl;
using CodeBase.Gameplay.VFX;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Gameplay.Skills.Factory.Impl {
    public class SkillFactory : ISkillFactory {
        private readonly IAssetProvider _assetProvider;
        private readonly IInstantiateService _instantiateService;
        private readonly CancellationToken _cancellationToken;

        public SkillFactory(
            IAssetProvider assetProvider,
            IInstantiateService instantiateService,
            CancellationToken cancellationToken
        ) {
            _assetProvider = assetProvider;
            _instantiateService = instantiateService;
            _cancellationToken = cancellationToken;
        }

        public async UniTask<ISkill> Create(ISkillConfig config, ASkillHolder holder) {
            return await Create((dynamic) config, holder);
        }

        private async UniTask<ISkill> Create(SlayAllConfig config, ASkillHolder holder) {
            var vfxPrefab = await _assetProvider.LoadAsset<GameObject>(config.VFXReference);
            _assetProvider.Release(config.VFXReference);
            
            _cancellationToken.ThrowIfCancellationRequested();
            
            return new SlayAllSkill(config.Radius, config.Delay, vfxPrefab, _instantiateService);
        }

        private async UniTask<ISkill> Create(RageConfig config, ASkillHolder holder) {
            var vfxPrefab = await _assetProvider.LoadAsset<GameObject>(config.VFXReference);
            _assetProvider.Release(config.VFXReference);
            
            _cancellationToken.ThrowIfCancellationRequested();
            
            var vfx = _instantiateService.Instantiate(vfxPrefab, holder.transform);
            return new RageSkill(config.Duration, config.Multiplier, vfx);
        }
        
        private async UniTask<ISkill> Create(HyperArmorConfig config, ASkillHolder holder) {
            await UniTask.Yield();
            return new HyperArmorSkill(config.Duration, holder.GetComponentInChildren<HyperArmorVfx>());
        }
    }
}