using System;
using CodeBase.Gameplay.Enemies.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Gameplay.Enemies.Spawners {
    public class EnemyDoorSpawnPoint : MonoBehaviour, IEnemySpawnPoint {
        [SerializeField] [Min(0f)] private float _appearDuration = 1f;
        [SerializeField] private Transform _origin;
        [SerializeField] private Transform _walkTo;

        private bool _spawning = false;

        public bool IsPointFree => Physics.OverlapBoxNonAlloc(_walkTo.position + Vector3.up * 0.5f, Vector3.one / 2.25f, new Collider[1]) == 0 && !_spawning;

        public void Spawn(GameObject enemyObject) {
            if(!IsPointFree) throw new Exception();
            
            enemyObject.Deactivate();
            enemyObject.GetComponent<NavMeshAgent>().Disable();
            enemyObject.GetComponent<EnemyBrain>().Disable();
            
            enemyObject.transform.rotation = _origin.rotation;
            enemyObject.transform.position = _origin.position;
            enemyObject.transform
                .DOMove(_walkTo.position, _appearDuration)
                .SetEase(Ease.InOutCirc)
                .OnComplete(() => {
                    enemyObject.GetComponent<NavMeshAgent>().Enable();
                    enemyObject.GetComponent<EnemyBrain>().Enable();
                });
            foreach (var enemyRenderer in enemyObject.GetComponentsInChildren<Renderer>()) {
                Material material = enemyRenderer.material;
                
                Color solidColor = material.color;
                Color transparentColor = solidColor;
                transparentColor.a = 0;

                material.color = transparentColor;
                material.DOColor(solidColor, _appearDuration).SetEase(Ease.InOutCirc);
            }
            
            enemyObject.Activate();
        }
    }
}