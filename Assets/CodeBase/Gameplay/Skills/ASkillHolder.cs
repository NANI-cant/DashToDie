using System;
using System.Collections.Generic;
using CodeBase.Gameplay.General;
using UnityEngine;

namespace CodeBase.Gameplay.Skills {
    [RequireComponent(typeof(ITeamProvider))]
    public abstract class ASkillHolder: MonoBehaviour, ICancelable {
        private Dictionary<int, ISkill> _skillsById = new();
        private Dictionary<Type, ISkill> _skillsByType = new();
        
        private bool _blocked;

        public int SkillsCount => _skillsById.Count;

        public event Action<int> SkillExecuted;

        public void Initialize(ISkill[] skills) {
            _skillsById = new Dictionary<int, ISkill>();
            _skillsByType = new Dictionary<Type, ISkill>();
            for (int i = 0; i <skills.Length; i++) {
                _skillsById.Add(i, skills[i]);
                _skillsByType.Add(skills[i].GetType(), skills[i]);
            }
        }

        public void ExecuteSkill(int id) {
            if (_blocked) return;
            if (id > SkillsCount - 1) 
                throw new ArgumentOutOfRangeException($"{GetType()} dont have skill with id = {id}");
            
            if (_skillsById[id].TryExecute(this, GetComponent<ITeamProvider>().TeamId)) {
                SkillExecuted?.Invoke(id);    
            }
        }

        public ISkill GetSkill(int id) {
            if (id > SkillsCount - 1) 
                throw new ArgumentOutOfRangeException($"{GetType()} dont have skill with id = {id}");
            return _skillsById[id];
        }

        public TSkill GetSkill<TSkill>() where TSkill: ISkill{
            return (TSkill) _skillsByType[typeof(TSkill)];
        }

        public void Cancel() {
            foreach (var skill in _skillsById.Values) {
                skill.Cancel();
            }
        }

        public void Block() => _blocked = true;
        public void Unblock() => _blocked = false;

        protected virtual void OnDestroy() {
            foreach (var skill in _skillsById.Values) {
                skill.Cancel();
            }
        }
    }
}