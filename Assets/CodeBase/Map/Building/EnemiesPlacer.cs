using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Gameplay.Enemies.Spawners;
using CodeBase.ProjectContext.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Map.Building {
    public class EnemiesPlacer {
        private readonly IRandomService _randomService;
        private readonly CancellationToken _cancelToken;

        public EnemiesPlacer(
            IRandomService randomService,
            CancellationToken cancelToken
        ) {
            _randomService = randomService;
            _cancelToken = cancelToken;
        }
        
        public void Place(GameObject[] enemiesObjects) {
            EnemyDoorSpawnPoint[] spawnPoints = Object.FindObjectsOfType<EnemyDoorSpawnPoint>();
            if (spawnPoints.Length == 0) throw new ArgumentNullException();

            Queue<GameObject> enemiesToPlace = new Queue<GameObject>(enemiesObjects);
            foreach (var enemyObject in enemiesObjects) {
                enemyObject.Deactivate();
            }
            
            SpawnEnemiesOnPoints(spawnPoints, enemiesToPlace).Forget();
        }

        private async UniTask SpawnEnemiesOnPoints(EnemyDoorSpawnPoint[] spawnPoints, Queue<GameObject> enemiesToPlace) {
            while (enemiesToPlace.Count > 0) {
                spawnPoints = _randomService.Shuffle(spawnPoints);
                int enemiesRemind = enemiesToPlace.Count;
                for (int i = 0; i < Mathf.Min(spawnPoints.Length/2, enemiesRemind); i++) {
                    var spawnPoint = spawnPoints[i];
                    if (!spawnPoint.IsPointFree) continue;

                    var enemyObject = enemiesToPlace.Dequeue();
                    spawnPoint.Spawn(enemyObject);
                    enemyObject.Activate();
                }

                await UniTask.Delay((int) (0.5f * 1000));
                if(_cancelToken.IsCancellationRequested) return;
            }
        }
    }
}