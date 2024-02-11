using System.Threading;
using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Skills;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Elements {
    public class SkillView: MonoBehaviour {
        [SerializeField][Min(0)] private int _skillId;
        [SerializeField] private TMP_Text _charges;

        private ISkill _targetSkill;
        private CancellationToken _destroyToken;
        private PlayerSkillHolder _skillHolder;

        [Inject]
        public void Construct(GameObject playerObject) {
            _skillHolder = playerObject.GetComponent<PlayerSkillHolder>();
        }

        private void OnEnable() {
            if (_targetSkill != null) _targetSkill.ChargeCounter.Changed += UpdateChargesCount;
        }

        private void OnDisable() {
            if (_targetSkill != null) _targetSkill.ChargeCounter.Changed -= UpdateChargesCount;
        }

        private void Start() {
            _destroyToken = this.GetCancellationTokenOnDestroy();
            InitAsync().Forget();
        }

        private async UniTask InitAsync() {
            await _skillHolder.WaitInitialization();
            if(_destroyToken.IsCancellationRequested) return;
            
            _targetSkill = _skillHolder.GetSkill(_skillId);
            _targetSkill.ChargeCounter.Changed += UpdateChargesCount;
            UpdateChargesCount();
        }

        private void UpdateChargesCount() {
            _charges.text = _targetSkill.ChargeCounter.Charges.ToString();
        }
    }
}