using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase {
    public static class UnityExtensions {
        public static bool IntersectsWithMany(this Bounds bounds, params Bounds[] others) {
            foreach (var other in others) {
                if (bounds.Intersects(other)) return true;
            }
            return false;
        }

        public static Vector3Int ToInt(this Vector3 vector3) 
            => new Vector3Int((int) vector3.x, (int) vector3.y, (int) vector3.z);

        public static Vector3Int Round(this Vector3 vector3) {
            return new Vector3((float) Math.Round(vector3.x), (float) Math.Round(vector3.y), (float) Math.Round(vector3.z)).ToInt();
        }

        public static Vector3Int[] ToScheme(this Bounds bounds) {
            bounds.extents = bounds.extents.ToInt();
            
            List<Vector3Int> cells = new();
            for (int z = (int)bounds.min.z; z <= (int)bounds.max.z; z++) {
                for (int x = (int)bounds.min.x; x <= (int)bounds.max.x; x++) {
                    for (int y = (int)bounds.min.y; y <= (int)bounds.max.y; y++) {
                        cells.Add(new Vector3Int(x, y, z));
                    }
                }    
            }

            return cells.ToArray();
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 to) => (to - @from).normalized;

        public static Vector3 To4Directions(this Vector3 direction) {
            float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);

            if (Mathf.Cos(angle * Mathf.Deg2Rad) >= Mathf.Cos(45 * Mathf.Deg2Rad)) return Vector3.forward;
            if (Mathf.Cos(angle * Mathf.Deg2Rad) <= Mathf.Cos(135 * Mathf.Deg2Rad)) return Vector3.back;
            
            if (Mathf.Sin(angle * Mathf.Deg2Rad) > Mathf.Sin(45 * Mathf.Deg2Rad)) return Vector3.right;
            if (Mathf.Sin(angle * Mathf.Deg2Rad) < Mathf.Sin(-45 * Mathf.Deg2Rad)) return Vector3.left;
            
            return Vector3.zero;
        }

        public static void Activate(this GameObject gameObject) => gameObject.SetActive(true);
        public static void Deactivate(this GameObject gameObject) => gameObject.SetActive(false);
        
        public static void Enable(this Behaviour behaviour) => behaviour.enabled = true;
        public static void Disable(this Behaviour behaviour) => behaviour.enabled = false;

        public static void Subscribe(this Button button, UnityAction action) => button.onClick.AddListener(action);
        public static void Unsubscribe(this Button button, UnityAction action) => button.onClick.RemoveListener(action);

        public static TComponent[] GetComponentFromAll<TComponent>(this IEnumerable<GameObject> gameObjects) {
            List<TComponent> foundComponents = new();
            foreach (var gameObject in gameObjects) {
                foundComponents.Add(gameObject.GetComponent<TComponent>());
            }
            
            return foundComponents.ToArray();
        }
        
        public static bool IsPrefab(this GameObject gameObject) => gameObject.scene.name == null;

        public static Plane GetPlane(this Transform transform) {
            var position = transform.position;
            return new Plane(position, position + transform.forward, position + transform.right);
        }

        public static Vector3 ToXZPlane(this Vector3 vector) {
            vector.y = 0;
            return vector;
        }

        public static bool ApproximateEqual(this Vector3 vector, Vector3 other, float accuracy) {
            return Mathf.Abs(vector.x - other.x) <= accuracy && 
                   Mathf.Abs(vector.y - other.y) <= accuracy &&
                   Mathf.Abs(vector.z - other.z) <= accuracy;
        }
    }
}