using UnityEngine;

namespace CodeBase.Gameplay.General {
    public interface IRotator {
        void RotateToDirection(Vector3 direction);
        void RotateToPoint(Vector3 point);
    }
}