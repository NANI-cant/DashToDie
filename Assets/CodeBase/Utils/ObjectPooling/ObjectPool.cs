using System.Collections.Generic;
using CodeBase.ProjectContext.Services;
using UnityEngine;

namespace CodeBase.Utils.ObjectPooling {
    public class ObjectPool {
        private readonly IInstantiateService _instantiateService;
        private readonly GameObject _template;
        private readonly Transform _container;

        private readonly Stack<GameObject> _inactiveObjects = new();
        private readonly List<GameObject> _activeObjects = new ();

        public ObjectPool(
            IInstantiateService instantiateService,
            int startSize, 
            GameObject template, 
            bool isTemplatePrefab,
            Transform container = null
        ) {
            _instantiateService = instantiateService;
            _template = template;
            _container = container;
            
            if (!isTemplatePrefab) {
                _template.SetActive(false);
                _template.transform.SetParent(container);
            }

            Expand(startSize);
        }

        public GameObject Get() {
            if(_inactiveObjects.Count == 0) Expand(_activeObjects.Count);

            var preparedObject = _inactiveObjects.Pop();
            _activeObjects.Add(preparedObject);
            preparedObject.SetActive(true);

            return preparedObject;
        }

        public void Return(GameObject returnableObject) {
            returnableObject.SetActive(false);
            _activeObjects.Remove(returnableObject);
            _inactiveObjects.Push(returnableObject);
        }

        private void Expand(int capacity) {
            for (int i = 0; i < capacity; i++) {
                GameObject newObject = _instantiateService.Instantiate(_template);
                newObject.SetActive(false);
                newObject.transform.SetParent(_container);
                
                _inactiveObjects.Push(newObject);
            }
        }
    }
}