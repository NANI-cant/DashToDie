using UnityEngine;

namespace CodeBase.Gameplay.General.Impl {
    public class TeamProvider : MonoBehaviour, ITeamProvider {
        [SerializeField] private int _teamId;

        public int TeamId => _teamId;
    }
}