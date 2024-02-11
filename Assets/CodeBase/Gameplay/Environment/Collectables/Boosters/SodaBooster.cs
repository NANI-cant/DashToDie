using System.Threading;
using CodeBase.Gameplay.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class SodaBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(0)] private float _staminaPriceMultiplier;
        [SerializeField] [Min(0)] private float _recoveryMultiplier;
        [SerializeField] [Min(0)] private int _duration;

        private CancellationToken _cancelToken; 

        [Inject]
        public void Construct(CancellationToken cancelToken) {
            _cancelToken = cancelToken;
        }

        public void Apply(GameObject target) {
            var charger = target.GetComponent<SlashCharger>();
            var recovery = target.GetComponent<SlashRecovery>();
            recovery.Recovery.Decrease(_recoveryMultiplier);
            charger.StaminaPrice.Decrease(_staminaPriceMultiplier);
            
            CancelAfterDelay(charger, recovery).Forget();
        }

        private async UniTask CancelAfterDelay(SlashCharger charger, SlashRecovery recovery) {
            await UniTask.Delay(_duration * 1000);
            if(_cancelToken.IsCancellationRequested) return;

            recovery.Recovery.Increase(_recoveryMultiplier);
            charger.StaminaPrice.Increase(_staminaPriceMultiplier);
        }
    }
}