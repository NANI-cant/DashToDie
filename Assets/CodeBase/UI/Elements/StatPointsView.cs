using CodeBase.Metaplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Elements {
    public class StatPointsView: MonoBehaviour {
        [SerializeField] private TMP_Text _text;
        
        private IStatPointsBank _statPointsBank;

        [Inject]
        public void Construct(IStatPointsBank statPointsBank) {
            _statPointsBank = statPointsBank;
        }

        private void Start() {
            UpdateView();
            _statPointsBank.Changed += UpdateView;
        }

        private void OnDestroy() => _statPointsBank.Changed -= UpdateView;

        private void UpdateView() {
            _text.text = _statPointsBank.Available.ToString();
        }
    }
}