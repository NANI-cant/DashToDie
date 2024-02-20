using System;
using UnityEngine;
using Zenject;

namespace CodeBase.ProjectContext.Services.Impl {
    public class DefaultInputService : IInputService, ITickable{
        public event Action SlashCalled;
        
        public event Action<int> SkillCalled;

        public bool Charging { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public Vector3 PointerPosition => Input.mousePosition;
        public Ray GetPointerRay(Camera camera) => camera.ScreenPointToRay(PointerPosition);

        public void Tick() {
            HandleSkills();

            Charging = Input.GetMouseButton(0);
            if(Input.GetMouseButtonUp(0)) SlashCalled?.Invoke();

            MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        private void HandleSkills() {
            if(Input.GetKeyDown(KeyCode.Alpha1)) SkillCalled?.Invoke(0);
            if(Input.GetKeyDown(KeyCode.Alpha2)) SkillCalled?.Invoke(1);
            if(Input.GetKeyDown(KeyCode.Alpha3)) SkillCalled?.Invoke(2);
            if(Input.GetKeyDown(KeyCode.Alpha4)) SkillCalled?.Invoke(3);
        }
    }
}