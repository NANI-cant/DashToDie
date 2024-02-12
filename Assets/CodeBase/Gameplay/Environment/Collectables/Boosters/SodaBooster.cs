using System.Threading;
using CodeBase.Gameplay.General.Brains.Impl;
using CodeBase.Gameplay.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Slasher = CodeBase.Gameplay.Player.Slasher;

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
            var slasher = target.GetComponent<Slasher>();
            slasher.Recovery.Decrease(_recoveryMultiplier);
            slasher.StaminaPrice.Decrease(_staminaPriceMultiplier);
            
            CancelAfterDelay(slasher).Forget();
        }

        private async UniTask CancelAfterDelay(Slasher slasher) {
            await UniTask.Delay(_duration * 1000);
            if(_cancelToken.IsCancellationRequested) return;

            slasher.Recovery.Increase(_recoveryMultiplier);
            slasher.StaminaPrice.Increase(_staminaPriceMultiplier);
        }
    }
}