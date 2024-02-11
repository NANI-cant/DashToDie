using UnityEngine;

namespace CodeBase.Gameplay.Enemies.Spawners {
    public interface IEnemySpawnPoint {
        bool IsPointFree { get; }
        void Spawn(GameObject enemyObject);
    }
}