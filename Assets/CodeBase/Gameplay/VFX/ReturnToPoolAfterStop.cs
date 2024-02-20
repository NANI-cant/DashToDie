using UnityEngine;
using UnityEngine.Pool;

namespace CodeBase.Gameplay.VFX {
    [RequireComponent(typeof(ParticleSystem))]
    public class ReturnToPoolAfterStop : MonoBehaviour {
        public IObjectPool<GameObject> Pool;

        void Start() {
            var main = GetComponent<ParticleSystem>().main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        void OnParticleSystemStopped() {
            Pool.Release(gameObject);
        }
    }
}