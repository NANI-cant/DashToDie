using UnityEngine;
using Zenject;

namespace CodeBase.ProjectContext.Services.Impl {
    public class ZenjectInstantiateService : IInstantiateService {
        private readonly IInstantiator _instantiator;

        public ZenjectInstantiateService(IInstantiator instantiator) {
            _instantiator = instantiator;
        }
        
        public GameObject Instantiate(GameObject prefab, Transform parent = null) {
            return _instantiator.InstantiatePrefab(prefab, parent);
        }
    }
}