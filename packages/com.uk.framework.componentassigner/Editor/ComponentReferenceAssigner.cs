using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.Editor.ComponentAssigner
{
    public class ComponentAssigner : EditorWindow
    {
        private GameObject parent;
        private MonoBehaviour targetComponent;

        private string componentTypeName;
        private string fieldName;

        [MenuItem("Tools/Component Assigner")]
        public static void Open()
        {
            GetWindow<ComponentAssigner>("Component Assigner");
        }

        private void OnGUI()
        {
            parent = (GameObject)EditorGUILayout.ObjectField(
                "Parent",
                parent,
                typeof(GameObject),
                true
            );

            targetComponent = (MonoBehaviour)EditorGUILayout.ObjectField(
                "Target Component",
                targetComponent,
                typeof(MonoBehaviour),
                true
            );

            componentTypeName = EditorGUILayout.TextField(
                "Component Type",
                componentTypeName
            );

            fieldName = EditorGUILayout.TextField(
                "Array / List Field",
                fieldName
            );

            GUILayout.Space(10);

            if (GUILayout.Button("Assign"))
            {
                Assign();
            }
        }

        private void Assign()
        {
            if (parent == null || targetComponent == null)
            {
                Debug.LogError("Parent or Target Component is missing.");
                return;
            }

            Type componentType = FindType(componentTypeName);

            if (componentType == null)
            {
                Debug.LogError(
                    $"Component '{componentTypeName}' not found."
                );
                return;
            }

            FieldInfo field = targetComponent.GetType().GetField(
                fieldName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            if (field == null)
            {
                Debug.LogError(
                    $"Field '{fieldName}' not found on " +
                    $"{targetComponent.GetType().Name}"
                );
                return;
            }


            // uk change this in future
            Component[] components =
                parent.GetComponentsInChildren(componentType);

            if (field.FieldType.IsArray)
            {
                Type elementType = field.FieldType.GetElementType();

                Array array = Array.CreateInstance(
                    elementType,
                    components.Length
                );

                for (int i = 0; i < components.Length; i++)
                {
                    array.SetValue(components[i], i);
                }

                field.SetValue(targetComponent, array);
            }
            else if (
                field.FieldType.IsGenericType &&
                field.FieldType.GetGenericTypeDefinition() == typeof(List<>)
            )
            {
                Type elementType =
                    field.FieldType.GetGenericArguments()[0];

                var list = Activator.CreateInstance(
                    typeof(List<>).MakeGenericType(elementType)
                ) as System.Collections.IList;

                foreach (Component component in components)
                {
                    list.Add(component);
                }

                field.SetValue(targetComponent, list);
            }
            else
            {
                Debug.LogError(
                    $"'{fieldName}' is not an array or List<T>."
                );

                return;
            }

            EditorUtility.SetDirty(targetComponent);

            Debug.Log(
                $"Assigned {components.Length} " +
                $"{componentType.Name} components to {fieldName}."
            );
        }

        private Type FindType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == typeName)
                        return type;
                }
            }

            return null;
        }
    }

}