using System;
using CodeBase.Gameplay.PlayerStats;
using CodeBase.Metaplay;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Elements {
    public class StatView: MonoBehaviour {
        [SerializeField] private AStatObject _stat;

        [Header("Setup")]
        [SerializeField] private LocalizeStringEvent _labelLocalize;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _minusButton;
        
        private IStatPointsBank _statPointsBank;

        [Inject]
        public void Construct(IStatPointsBank statPointsBank) {
            _statPointsBank = statPointsBank;
        }

        private void OnEnable() {
            _plusButton.Subscribe(Increase);
            _minusButton.Subscribe(Decrease);
            _statPointsBank.Changed += UpdateUI;
        }

        private void OnDisable() {
            _plusButton.Unsubscribe(Increase);
            _minusButton.Unsubscribe(Decrease);
            _statPointsBank.Changed -= UpdateUI;
        }

        public void Initialize(AStatObject aStatObject) {
            _stat = aStatObject;
            gameObject.name = _stat.name + "View";
            _labelLocalize.StringReference = _stat.LocalizedString;
            _level.text = _stat.Level.ToString();
        }

        private void Start() {
            Initialize(_stat);
            UpdateUI();
        }

        private void Increase() {
            _statPointsBank.Take(1);
            _stat.Level++;
            UpdateUI();
        }

        private void Decrease() {
            _stat.Level--;
            _statPointsBank.Put(1);
            UpdateUI();
        }

        private void UpdateUI() {
            _plusButton.interactable = _stat.Level < _stat.MaxLevel && _statPointsBank.Available != 0;
            _minusButton.interactable = _stat.Level > 0;
            _level.text = _stat.Level.ToString();
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (_stat == null) return;
            
            try {
                Initialize(_stat);
            }
            catch (Exception) {
                // ignored
            }
        }
#endif
    }
}