using System.Collections.Generic;
using CodeBase.Gameplay.General.Brains.Impl;
using CodeBase.Gameplay.Player;
using UnityEngine;

namespace CodeBase.Gameplay.VFX {
    public class SlasherChargingVFX : MonoBehaviour {
        [SerializeField] private Slasher _slasher;
        [SerializeField] private Material _material;
        [SerializeField] private SkinnedMeshRenderer[] _meshRenderers;
        [SerializeField] private Transform _centerOrigin;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _obstacles;

        private readonly List<MeshFilter> _avatarFilters = new();
        
        private Transform _avatar;

        private void Start() {
            PrepareAvatar();
        }

        private void LateUpdate() {
            bool showAvatar = _slasher.ChargedDistance > 0 && _slasher.enabled;
            
            _lineRenderer.positionCount = showAvatar ? 2 : 0;
            _avatar.gameObject.SetActive(showAvatar);
            
            if (showAvatar) {
                UpdateAvatarMesh();
                var slashDistance = _slasher.ChargedDistance;
                if (Physics.Raycast(_slasher.SlashOrigin.position, _slasher.SlashOrigin.forward, out var raycastHit, slashDistance, _obstacles)) {
                    slashDistance = raycastHit.distance;
                }

                _avatar.position = transform.position + _slasher.SlashOrigin.forward * slashDistance;
                
                _lineRenderer.SetPosition(0, _centerOrigin.localPosition);
                _lineRenderer.SetPosition(1, _centerOrigin.localPosition + Vector3.forward * slashDistance);
            }
        }

        private void PrepareAvatar() {
            GameObject avatarGO = new GameObject("SlashAvatar");
            _avatar = avatarGO.transform;
            _avatar.transform.SetParent(transform);

            foreach (var meshRenderer in _meshRenderers) {
                var part = new GameObject(meshRenderer.gameObject.name);
                part.transform.SetParent(_avatar);
                part.transform.rotation *= Quaternion.Euler(_rotation);

                var partRenderer = part.AddComponent<MeshRenderer>();
                var partFilter = part.AddComponent<MeshFilter>();
                _avatarFilters.Add(partFilter);

                partRenderer.material = _material;
            }

            avatarGO.SetActive(false);
        }

        private void UpdateAvatarMesh() {
            for (int i = 0; i < _meshRenderers.Length; i++) {
                var bakedMesh = new Mesh();
                _meshRenderers[i].BakeMesh(bakedMesh);

                _avatarFilters[i].mesh = bakedMesh;
            }
        }
    }
}