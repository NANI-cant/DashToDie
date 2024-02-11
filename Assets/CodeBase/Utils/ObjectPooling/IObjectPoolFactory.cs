using UnityEngine;

namespace CodeBase.Utils.ObjectPooling {
    public interface IObjectPoolFactory {
        public ObjectPool Create(
            int startSize, 
            GameObject template, 
            bool isTemplatePrefab,
            Transform container = null);
    }
}