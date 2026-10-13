
using System.Collections.Generic;
using UnityEngine;

namespace UKFramework.Game
{
    public class ObjectPool
    {
        private readonly GameObject prefab;
        private readonly Transform container;

        private readonly Queue<GameObject> objects = new();

        // Tracks objects currently available in the pool.
        private readonly HashSet<GameObject> available = new();

        // Tracks objects currently active in the pool.
        private readonly HashSet<GameObject> activeObjects = new();

        public ObjectPool(GameObject prefab, Transform container, int size)
        {
            this.prefab = prefab;
            this.container = container;

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Object.Instantiate(prefab, container);

                obj.SetActive(false);
                objects.Enqueue(obj);
                available.Add(obj);
            }
        }

        public GameObject Get()
        {
            GameObject obj = null;

            // Skip objects destroyed outside the pool.
            while (objects.Count > 0)
            {
                GameObject candidate = objects.Dequeue();

                available.Remove(candidate);
                activeObjects.Add(candidate);

                if (candidate == null)
                    continue;

                obj = candidate;
                break;
            }

            // Create a replacement when no valid object exists.
            if (obj == null)
            {
                obj = Object.Instantiate(prefab, container);
            }

            obj.SetActive(true);

            return obj;
        }

        public void Return(GameObject obj)
        {
            // Destroyed or missing object.
            if (obj == null)
                return;

            // Prevent duplicate pool returns.
            if (!available.Add(obj))
            {
                Debug.LogWarning(
                    $"ObjectPool: '{obj.name}' " +
                    "was already returned to the pool.");

                return;
            }

            obj.SetActive(false);

            obj.transform.SetParent(container);
            activeObjects.Remove(obj);
            objects.Enqueue(obj);
        }

        public void GetActiveObjectsWithComponent<T>(List<GameObject> results)
             where T : Component
        {
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null && obj.activeInHierarchy && obj.TryGetComponent<T>(out _))
                {
                    results.Add(obj);
                }
            }
        }
    }
}
