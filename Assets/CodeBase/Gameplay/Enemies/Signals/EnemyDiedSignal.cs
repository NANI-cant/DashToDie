using UnityEngine;

namespace CodeBase.Gameplay.Enemies.Signals {
    public class EnemyDiedSignal {
        public GameObject EnemyObject { get; }

        public EnemyDiedSignal(GameObject enemyObject) {
            EnemyObject = enemyObject;
        }
    }
}