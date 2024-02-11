using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Debug {
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIAgentDebug: MonoBehaviour {
        [SerializeField] private NavMeshSurface _surface;
        [SerializeField] private Transform _follow;
        
        private NavMeshAgent _agent;

        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            BuildMesh();
        }

        private void Update() {
            if(_follow == null) return;

            _agent.destination = _follow.position;
        }

        [ContextMenu("BuildNavMesh")]
        public void BuildMesh() {
            _surface.BuildNavMesh();
        }
    }
}