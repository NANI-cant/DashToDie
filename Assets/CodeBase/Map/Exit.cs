using System;
using CodeBase.Gameplay.Environment.Collectables;
using UnityEngine;
using Zenject;

namespace CodeBase.Map {
    [RequireComponent(typeof(Collider))]
    public class Exit: MonoBehaviour, ICollectable {
        private Collider _trigger;
        private GameObject _playerObject;

        public event Action PlayerExited;

        [Inject]
        public void Construct(GameObject playerObject) {
            _playerObject = playerObject;
        }

        private void Awake() => _trigger = GetComponent<Collider>();
        private void Start() => _trigger.isTrigger = true;

        public void Collect(GameObject collector) {
            if (collector != _playerObject) return;

            _trigger.enabled = false;
            PlayerExited?.Invoke();
        }
    }
}