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




        public GameObject Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation)
        {
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(
                prefab,
                10);

            }

            GameObject obj =
            pools[prefab].Get();

            obj.transform.position =
            position;

            obj.transform.rotation =
            rotation;

            return obj;

        }

        public void Despawn(
        GameObject prefab,
        GameObject obj)
        {
            pools[prefab].Return(obj);

        }

    }

}