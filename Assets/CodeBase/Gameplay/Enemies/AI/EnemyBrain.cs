using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rotator))]
    public class EnemyBrain : MonoBehaviour {
        protected NavMeshAgent Agent { get; private set; }
        protected Rotator Rotator { get; private set; }

        protected virtual void Awake() {
            Agent = GetComponent<NavMeshAgent>();
            Rotator = GetComponent<Rotator>();
        }
    }
}