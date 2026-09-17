using UnityEngine;
using System.Collections.Generic;


namespace UKFramework.Game
{
    public class ObjectPool
    {
        private GameObject prefab;

        private Transform container;

        private Queue<GameObject> objects =
        new();


        public ObjectPool(
        GameObject prefab,
        Transform container,
        int size)
        {

            this.prefab = prefab;

            this.container = container;



            for (int i = 0; i < size; i++)
            {

                GameObject obj =
                Object.Instantiate(
                prefab,
                container);


                obj.SetActive(false);


                objects.Enqueue(obj);

            }


        }



        public GameObject Get()
        {

            GameObject obj;



            if (objects.Count > 0)
            {

                obj = objects.Dequeue();

            }

            else
            {

                obj =
                Object.Instantiate(
                prefab,
                container);

            }



            obj.SetActive(true);


            return obj;

        }




        public void Return(
        GameObject obj)
        {

            obj.SetActive(false);


            obj.transform.SetParent(
            container);


            objects.Enqueue(obj);

        }


    }

}