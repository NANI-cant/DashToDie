using UnityEngine;

namespace CodeBase.ProjectContext.Services {
    public interface IInstantiateService {
        GameObject Instantiate(GameObject prefab, Transform parent = null);
    }
}