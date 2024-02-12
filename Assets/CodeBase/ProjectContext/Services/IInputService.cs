using System;
using UnityEngine;
using Zenject;

namespace CodeBase.ProjectContext.Services {
    public interface IInputService{
        event Action SlashCharged;
        event Action SlashCalled;
        event Action<int> SkillCalled;
        
        bool Charging { get;}
        Vector3 MoveDirection { get; }
        Vector3 PointerPosition { get; }

        Ray GetPointerRay(Camera camera);
    }
}