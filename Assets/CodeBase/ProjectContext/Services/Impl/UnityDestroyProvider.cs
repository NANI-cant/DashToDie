using UnityEngine;

namespace CodeBase.ProjectContext.Services.Impl {
    public class UnityDestroyProvider : IDestroyProvider {
        public void Destroy(GameObject gameObject, float delay = 0) 
            => GameObject.Destroy(gameObject, delay);
    }
}