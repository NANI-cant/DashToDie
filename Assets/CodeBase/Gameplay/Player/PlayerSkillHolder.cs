using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.Skills;
using CodeBase.Gameplay.Skills.Configs;
using CodeBase.Gameplay.Skills.Factory;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(IDamageDealer))]
    [RequireComponent(typeof(ITeamProvider))]
    public class PlayerSkillHolder : ASkillHolder {
        [SerializeField] private AssetReference[] _skillsReferences;

        private IInputService _input;
        private ISkillFactory _skillFactory;
        private IAssetProvider _assetProvider;
        private CancellationToken _destroyCancel;
        private bool _initialized;

        [Inject]
        public void Construct(
            IInputService inputService,
            IInstantiateService instantiateService,
            IAssetProvider assetProvider,
            ISkillFactory skillFactory
        ) {
            _input = inputService;
            _skillFactory = skillFactory;
            _assetProvider = assetProvider;
            _destroyCancel = this.GetCancellationTokenOnDestroy();
            
            InitAsync().Forget();
        }

        public async UniTask WaitInitialization() {
            while (!_initialized) {
                await UniTask.Yield();
                if(_destroyCancel.IsCancellationRequested) return;
            }
        }

        private async UniTaskVoid InitAsync() {
            ISkillConfig[] configs = await _assetProvider.LoadAssets<ISkillConfig>(_skillsReferences);
            _assetProvider.Release(_skillsReferences);
            if(_destroyCancel.IsCancellationRequested) return;
            
            ISkill[] skills = new ISkill[configs.Length];
            for (int i = 0; i < configs.Length; i++) {
                var skill = await _skillFactory.Create(configs[i], this);
                skills[i] = skill;
            }
            Initialize(skills);
            
            _initialized = true;
        }

        private void OnEnable() => _input.SkillCalled += ExecuteSkill;
        private void OnDisable() => _input.SkillCalled -= ExecuteSkill;
    }
}