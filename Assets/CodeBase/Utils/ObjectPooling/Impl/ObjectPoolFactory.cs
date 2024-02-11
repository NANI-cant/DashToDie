using CodeBase.ProjectContext.Services;
using UnityEngine;

namespace CodeBase.Utils.ObjectPooling.Impl {
    public class ObjectPoolFactory : IObjectPoolFactory {
        private readonly IInstantiateService _instantiateService;

        public ObjectPoolFactory(IInstantiateService instantiateService) {
            _instantiateService = instantiateService;
        }
        
        public ObjectPool Create(int startSize, GameObject template, bool isTemplatePrefab, Transform container = null) {
            return new ObjectPool(_instantiateService, startSize, template, isTemplatePrefab, container);
        }
    }
}