using UnityEngine;

namespace UKFramework.Game
{
    public class ObjectFactory
    {
        // public GameObject Create(ObjectDefinition definition, Vector3 position, Quaternion rotation)
        // {
        //     return Object.Instantiate(definition.prefab, position, rotation);

        // }
        public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(prefab, position, rotation);

        }
    }

}