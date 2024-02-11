using CodeBase.Metaplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Elements {
    public class BankView: MonoBehaviour {
        [SerializeField] private TMP_Text _text;
        
        private IMoneyBank _moneyBank;

        [Inject]
        public void Construct(IMoneyBank moneyBank) {
            _moneyBank = moneyBank;
        }

        private void Start() {
            UpdateView();
            _moneyBank.Changed += UpdateView;
        }

        private void OnDestroy() => _moneyBank.Changed -= UpdateView;

        private void UpdateView() {
            _text.text = _moneyBank.Amount.ToString();
        }
    }
}