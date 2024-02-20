using UnityEngine;

namespace CodeBase.Gameplay.VFX {
    public class HyperArmorVfx: MonoBehaviour {
        [SerializeField] private Renderer _renderer;
        
        private void OnEnable() {
            _renderer.material.EnableKeyword("_EMISSION");
        }

        private void OnDisable() {
            _renderer.material.DisableKeyword("_EMISSION");
        }
    }
}