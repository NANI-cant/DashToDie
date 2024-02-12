using System.Threading;
using CodeBase.Gameplay.General.Impl;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class HyperArmorBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(0)] private int _duration;

        private CancellationToken _cancelToken;

        [Inject]
        public void Construct(CancellationToken cancelToken) {
            _cancelToken = cancelToken;
        }

        public void Apply(GameObject target) {
            var hurtProcessor = target.GetComponent<HurtProcessor>();
            hurtProcessor._resistance += 1;

            CancelAfterDelay(hurtProcessor).Forget();
        }

        private async UniTask CancelAfterDelay(HurtProcessor target) {
            await UniTask.Delay(_duration * 1000);
            if(_cancelToken.IsCancellationRequested) return;

            target._resistance -= 1;
        }
    }
}