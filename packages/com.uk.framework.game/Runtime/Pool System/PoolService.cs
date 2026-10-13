using System.Collections.Generic;
using UnityEngine;

namespace UKFramework.Game
{
    public class PoolService
    {
        private Dictionary<GameObject, ObjectPool> pools = new();

        private Transform container;
        public PoolService()
        {
            GameObject root = new GameObject("PoolContainer");
            container = root.transform;
            Object.DontDestroyOnLoad(root);

        }

        public void CreatePool(GameObject prefab, int size)
        {
            ObjectPool pool = new ObjectPool(prefab, container, size);
            pools.Add(prefab, pool);

        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab, 10);
            }

            GameObject obj = pools[prefab].Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            return obj;

        }

        public void Despawn(GameObject prefab, GameObject obj)
        {
            pools[prefab].Return(obj);

        }

        // public void GetActiveComponents<T>(List<T> results)
        //     where T : class
        // {
        //     foreach (GameObject obj in pools)
        //     {
        //         if (!obj.activeInHierarchy)
        //             continue;

        //         if (obj.TryGetComponent<T>(out T component))
        //             results.Add(component);
        //     }
        // }


        public void DespawnActiveObjectsWithComponent<T>()
            where T : Component
        {
            List<GameObject> matches = new();
            foreach (ObjectPool pool in pools.Values)
            {
                matches.Clear();
                pool.GetActiveObjectsWithComponent<T>(matches);

                foreach (GameObject obj in matches)
                {
                    pool.Return(obj);
                }
            }
            matches.Clear();
        }

    }

}