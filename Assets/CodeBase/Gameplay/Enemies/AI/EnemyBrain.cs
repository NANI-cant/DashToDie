using CodeBase.Gameplay.General.Brains;
using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rotator))]
    public class EnemyBrain : MonoBehaviour, IBrain {
        protected NavMeshAgent Agent { get; private set; }
        protected Rotator Rotator { get; private set; }
        
        public void Enable() => enabled = true;
        public void Disable() => enabled = false;

        protected virtual void Awake() {
            Agent = GetComponent<NavMeshAgent>();
            Rotator = GetComponent<Rotator>();
        }
    }
}