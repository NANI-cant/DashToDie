using System;
using CodeBase.ProjectContext.GameStateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Screens {
    public class MainMenu: MonoBehaviour, IScreen {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _statsButton;

        [Space] 
        [SerializeField] private AssetReference _gameplayScene; 
        
        [Space]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField][Min(0)] private float _fadeDuration = 0.1f;

        private Tween _openHideTween;
        private GameStateMachine _gameStateMachine;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine) {
            _gameStateMachine = gameStateMachine;
        }

        private void OnEnable() {
            _playButton.Subscribe(Play);
            _shopButton.Subscribe(OpenShop);
            _statsButton.Subscribe(OpenStats);
        }

        private void OnDisable() {
            _playButton.Unsubscribe(Play);
            _shopButton.Unsubscribe(OpenShop);
            _statsButton.Unsubscribe(OpenStats);
        }

        private void OnDestroy() {
            _openHideTween?.Complete();
            _openHideTween?.Kill();
        }

        public void Open() {
            _openHideTween?.Complete();
            _openHideTween = _canvasGroup
                .DOFade(1, _fadeDuration)
                .SetEase(Ease.OutCirc)
                .OnPlay(() => {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                });
        }

        public void Hide() {
            _openHideTween?.Complete();
            _openHideTween = _canvasGroup
                .DOFade(1, _fadeDuration)
                .SetEase(Ease.InCirc)
                .OnPlay(() => {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                });
        }
        
        
        private void OpenStats() { }

        private void OpenShop() { }

        private void Play() {
            _gameStateMachine.TranslateTo<SceneLoadState>(_gameplayScene);
        }
    }
}
