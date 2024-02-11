using UnityEngine;

namespace CodeBase.ProjectContext.Services {
    public interface IDestroyProvider {
        void Destroy(GameObject gameObject, float delay = 0);
    }
}