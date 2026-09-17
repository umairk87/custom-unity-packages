using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UKFramework.Game.Editor
{
    public class LevelEditorWindowDesign : EditorWindow
    {
        private enum PlacementMode
        {
            Click,
            Drag
        }

        private readonly List<Type> componentTypes = new();
        private readonly List<GameObject> matchingPrefabs = new();

        private Type selectedComponentType;
        private GameObject selectedPrefab;
        private GameObject placementRoot;

        private PlacementMode placementMode =
            PlacementMode.Click;

        private GameObject preview;
        private Component previewComponent;
        private BoxCollider previewBoxCollider;

        private bool isDragging;

        private Vector3 lastPlacedPosition;
        private bool hasLastPlacedPosition;

        private const float PlacementDistance = 0.001f;
        private const float EdgeSnapDistance = 0.05f;
        private bool generationStopped;

        [MenuItem("Tools/Game/Level Editor Prefab Spawning")]
        private static void Open()
        {
            GetWindow<LevelEditorWindowDesign>(
                "Level Editor");
        }

        private void OnEnable()
        {
            RefreshComponentTypes();

            SceneView.duringSceneGui +=
                OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -=
                OnSceneGUI;

            DestroyPreview();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField(
                "Level Editor",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(8);

            DrawComponentSelector();

            EditorGUILayout.Space(8);

            DrawPrefabSelector();

            EditorGUILayout.Space(8);

            placementRoot =
                (GameObject)EditorGUILayout.ObjectField(
                    "Placement Root",
                    placementRoot,
                    typeof(GameObject),
                    true);

            EditorGUILayout.Space(8);

            placementMode =
                (PlacementMode)EditorGUILayout.EnumPopup(
                    "Placement Mode",
                    placementMode);

            EditorGUILayout.Space(12);

            if (GUILayout.Button(
                    "Refresh Prefabs",
                    GUILayout.Height(28)))
            {
                SearchProjectPrefabs();
            }
            if (generationStopped)
            {
                if (GUILayout.Button("Start Generation", GUILayout.Height(28)))
                {
                    StartGeneration();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Generation", GUILayout.Height(28)))
                {
                    StopGeneration();
                }
            }

            EditorGUILayout.Space(8);

            EditorGUILayout.HelpBox(
                "Click a collider in the Scene View to snap the selected prefab directly against its face. No grid is used.",
                MessageType.Info);

            if (selectedPrefab != null)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "Selected Prefab",
                    selectedPrefab.name);

                if (preview != null)
                {
                    EditorGUILayout.LabelField(
                        "Preview",
                        "Active");
                }
            }
        }

        private void DrawComponentSelector()
        {
            if (componentTypes.Count == 0)
                RefreshComponentTypes();

            string[] names =
                new string[componentTypes.Count];

            for (int i = 0; i < componentTypes.Count; i++)
            {
                names[i] =
                    componentTypes[i].Name;
            }

            int selectedIndex =
                selectedComponentType == null
                    ? 0
                    : Mathf.Max(
                        0,
                        componentTypes.IndexOf(
                            selectedComponentType));

            int newIndex =
                EditorGUILayout.Popup(
                    "Component",
                    selectedIndex,
                    names);

            if (newIndex >= 0 &&
                newIndex < componentTypes.Count)
            {
                Type newType =
                    componentTypes[newIndex];

                if (newType != selectedComponentType)
                {
                    selectedComponentType =
                        newType;

                    selectedPrefab = null;

                    SearchProjectPrefabs();

                    DestroyPreview();
                }
            }
        }
        private void StartGeneration()
        {
            generationStopped = false;

            EnsurePreview();

            SceneView.RepaintAll();
            Repaint();
        }
        private void StopGeneration()
        {
            generationStopped = true;
            isDragging = false;
            hasLastPlacedPosition = false;

            DestroyPreview();

            SceneView.RepaintAll();
            Repaint();
        }

        private void DrawPrefabSelector()
        {
            if (matchingPrefabs.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No prefabs found with the selected component and a BoxCollider on the same GameObject.",
                    MessageType.Warning);

                return;
            }

            string[] names =
                new string[matchingPrefabs.Count];

            for (int i = 0; i < matchingPrefabs.Count; i++)
            {
                names[i] =
                    matchingPrefabs[i].name;
            }

            int selectedIndex =
                selectedPrefab == null
                    ? 0
                    : Mathf.Max(
                        0,
                        matchingPrefabs.IndexOf(
                            selectedPrefab));

            int newIndex =
                EditorGUILayout.Popup(
                    "Prefab",
                    selectedIndex,
                    names);

            if (newIndex >= 0 &&
                newIndex < matchingPrefabs.Count)
            {
                GameObject newPrefab =
                    matchingPrefabs[newIndex];

                if (newPrefab != selectedPrefab)
                {
                    selectedPrefab =
                        newPrefab;

                    DestroyPreview();
                }
            }
        }

        private void RefreshComponentTypes()
        {
            componentTypes.Clear();

            TypeCache.TypeCollection types =
      TypeCache.GetTypesDerivedFrom<Component>();

            foreach (Type type in types)
            {
                if (type.IsAbstract)
                    continue;

                if (type.IsGenericType)
                    continue;

                componentTypes.Add(type);
            }

            componentTypes.Sort(
                (a, b) =>
                    string.Compare(
                        a.Name,
                        b.Name,
                        StringComparison.Ordinal));

            if (selectedComponentType == null &&
                componentTypes.Count > 0)
            {
                selectedComponentType =
                    componentTypes[0];

                SearchProjectPrefabs();
            }
        }

        private void SearchProjectPrefabs()
        {
            matchingPrefabs.Clear();

            if (selectedComponentType == null)
                return;

            string[] prefabGuids =
                AssetDatabase.FindAssets(
                    "t:Prefab",
                    new[] { "Assets" });

            foreach (string guid in prefabGuids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(
                        guid);

                GameObject prefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        path);

                if (prefab == null)
                    continue;

                Component component =
                    prefab.GetComponentInChildren(
                        selectedComponentType,
                        true);

                if (component == null)
                    continue;

                BoxCollider boxCollider =
                    component.GetComponent<BoxCollider>();

                if (boxCollider == null)
                    continue;

                matchingPrefabs.Add(prefab);
            }

            matchingPrefabs.Sort(
                (a, b) =>
                    string.Compare(
                        a.name,
                        b.name,
                        StringComparison.Ordinal));

            Repaint();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (generationStopped)
                return;
            Event currentEvent =
                Event.current;

            if (selectedPrefab == null)
                return;

            if (selectedComponentType == null)
                return;

            EnsurePreview();

            if (preview == null ||
                previewBoxCollider == null)
                return;

            Vector2 mousePosition =
                currentEvent.mousePosition;

            if (TryGetPlacement(
                    mousePosition,
                    out Vector3 position,
                    out Quaternion rotation))
            {
                preview.transform.SetPositionAndRotation(
                    position,
                    rotation);
            }

            HandleUtility.AddDefaultControl(
                GUIUtility.GetControlID(
                    FocusType.Passive));

            if (currentEvent.type ==
                EventType.MouseDown &&
                currentEvent.button == 0 &&
                !currentEvent.alt)
            {
                if (placementMode ==
                    PlacementMode.Click)
                {
                    PlacePrefab();

                    currentEvent.Use();
                }
                else
                {
                    isDragging = true;

                    PlacePrefab();

                    currentEvent.Use();
                }
            }

            if (currentEvent.type ==
                EventType.MouseDrag &&
                currentEvent.button == 0 &&
                isDragging &&
                !currentEvent.alt)
            {
                PlacePrefab();

                currentEvent.Use();
            }

            if (currentEvent.type ==
                EventType.MouseUp &&
                currentEvent.button == 0)
            {
                isDragging = false;

                currentEvent.Use();
            }

            sceneView.Repaint();
        }

        private void EnsurePreview()
        {
            if (preview != null)
            {
                if (preview.name !=
                    selectedPrefab.name +
                    "_Preview")
                {
                    DestroyPreview();
                }
                else
                {
                    return;
                }
            }

            preview =
                Instantiate(
                    selectedPrefab);

            preview.name =
                selectedPrefab.name +
                "_Preview";

            preview.hideFlags =
                HideFlags.HideAndDontSave;

            previewComponent =
                preview.GetComponentInChildren(
                    selectedComponentType,
                    true);

            if (previewComponent == null)
            {
                DestroyPreview();
                return;
            }

            previewBoxCollider =
                previewComponent.GetComponent<BoxCollider>();

            if (previewBoxCollider == null)
            {
                DestroyPreview();
                return;
            }

            /*
             * Disable every collider in the preview.
             *
             * This prevents the preview itself from
             * being detected by the Scene View raycast.
             */
            Collider[] colliders =
                preview.GetComponentsInChildren<Collider>(
                    true);

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }

        private void DestroyPreview()
        {
            if (preview != null)
            {
                DestroyImmediate(preview);
            }

            preview = null;
            previewComponent = null;
            previewBoxCollider = null;
        }

        // ============================================================
        // FACE-TO-FACE PLACEMENT
        // ============================================================

        private bool TryGetPlacement(
            Vector2 mousePosition,
            out Vector3 position,
            out Quaternion rotation)
        {
            position = Vector3.zero;

            rotation =
                selectedPrefab.transform.rotation;

            if (previewBoxCollider == null)
                return false;

            Ray mouseRay =
                HandleUtility.GUIPointToWorldRay(
                    mousePosition);

            if (!Physics.Raycast(
                    mouseRay,
                    out RaycastHit mouseHit,
                    Mathf.Infinity))
            {
                return false;
            }

            Collider targetCollider =
                mouseHit.collider;

            if (targetCollider == null)
                return false;

            BoxCollider targetBox =
                targetCollider as BoxCollider;

            if (targetBox == null)
            {
                return TryGetSurfacePlacement(
                    mouseHit,
                    out position,
                    out rotation);
            }

            /*
             * Keep the prefab's authored rotation.
             */
            rotation =
                selectedPrefab.transform.rotation;

            /*
             * Put preview at origin temporarily.
             * This gives us the actual world-space
             * collider geometry for the prefab.
             */
            preview.transform.SetPositionAndRotation(
                Vector3.zero,
                rotation);

            Vector3[] targetCorners =
                GetWorldBoxCorners(
                    targetBox);

            Vector3[] newCorners =
                GetWorldBoxCorners(
                    previewBoxCollider);

            Vector3 targetCenter =
                GetAverage(targetCorners);

            Vector3 newCenter =
                GetAverage(newCorners);

            Vector3 normal =
                mouseHit.normal.normalized;

            /*
             * Find which world axis the clicked face belongs to.
             */
            float absX =
                Mathf.Abs(normal.x);

            float absY =
                Mathf.Abs(normal.y);

            float absZ =
                Mathf.Abs(normal.z);

            Vector3 candidate =
                Vector3.zero;

            // ========================================================
            // Y FACE
            // ========================================================

            if (absY >= absX &&
                absY >= absZ)
            {
                if (normal.y > 0f)
                {
                    /*
                     * Target TOP
                     *
                     * New BOTTOM -> Target TOP
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.Y,
                            true);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.Y,
                            false);

                    candidate.y =
                        targetFace -
                        newFace;
                }
                else
                {
                    /*
                     * Target BOTTOM
                     *
                     * New TOP -> Target BOTTOM
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.Y,
                            false);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.Y,
                            true);

                    candidate.y =
                        targetFace -
                        newFace;
                }

                /*
                 * Follow mouse along the surface.
                 */
                candidate.x =
                    mouseHit.point.x -
                    newCenter.x;

                candidate.z =
                    mouseHit.point.z -
                    newCenter.z;

                /*
                 * Automatically align to nearby
                 * target edges.
                 */
                candidate =
                    SnapAlongTargetEdges(
                        candidate,
                        newCorners,
                        targetCorners,
                        mouseHit.point,
                        Axis.Y);
            }

            // ========================================================
            // X FACE
            // ========================================================

            else if (absX >= absY &&
                     absX >= absZ)
            {
                if (normal.x > 0f)
                {
                    /*
                     * Target RIGHT
                     *
                     * New LEFT -> Target RIGHT
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.X,
                            true);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.X,
                            false);

                    candidate.x =
                        targetFace -
                        newFace;
                }
                else
                {
                    /*
                     * Target LEFT
                     *
                     * New RIGHT -> Target LEFT
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.X,
                            false);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.X,
                            true);

                    candidate.x =
                        targetFace -
                        newFace;
                }

                candidate.y =
                    mouseHit.point.y -
                    newCenter.y;

                candidate.z =
                    mouseHit.point.z -
                    newCenter.z;

                candidate =
                    SnapAlongTargetEdges(
                        candidate,
                        newCorners,
                        targetCorners,
                        mouseHit.point,
                        Axis.X);
            }

            // ========================================================
            // Z FACE
            // ========================================================

            else
            {
                if (normal.z > 0f)
                {
                    /*
                     * Target BACK
                     *
                     * New FRONT -> Target BACK
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.Z,
                            true);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.Z,
                            false);

                    candidate.z =
                        targetFace -
                        newFace;
                }
                else
                {
                    /*
                     * Target FRONT
                     *
                     * New BACK -> Target FRONT
                     */
                    float targetFace =
                        GetExtremeCoordinate(
                            targetCorners,
                            Axis.Z,
                            false);

                    float newFace =
                        GetExtremeCoordinate(
                            newCorners,
                            Axis.Z,
                            true);

                    candidate.z =
                        targetFace -
                        newFace;
                }

                candidate.x =
                    mouseHit.point.x -
                    newCenter.x;

                candidate.y =
                    mouseHit.point.y -
                    newCenter.y;

                candidate =
                    SnapAlongTargetEdges(
                        candidate,
                        newCorners,
                        targetCorners,
                        mouseHit.point,
                        Axis.Z);
            }

            position = candidate;

            return true;
        }

        // ============================================================
        // EDGE SNAP
        // ============================================================

        private Vector3 SnapAlongTargetEdges(
            Vector3 candidate,
            Vector3[] newCorners,
            Vector3[] targetCorners,
            Vector3 mousePoint,
            Axis contactAxis)
        {
            GetBounds(
                targetCorners,
                out Vector3 targetMin,
                out Vector3 targetMax);

            /*
             * Calculate the new collider's size
             * in world space.
             */
            GetBounds(
                newCorners,
                out Vector3 newMin,
                out Vector3 newMax);

            Vector3 newSize =
                newMax - newMin;

            /*
             * Because the preview was calculated at
             * position zero, calculate the actual
             * center offset from its transform.
             */
            Vector3 newCenter =
                GetAverage(newCorners);

            float left =
                candidate.x +
                newMin.x;

            float right =
                candidate.x +
                newMax.x;

            float bottom =
                candidate.y +
                newMin.y;

            float top =
                candidate.y +
                newMax.y;

            float back =
                candidate.z +
                newMin.z;

            float front =
                candidate.z +
                newMax.z;

            /*
             * Y contact means we can snap X/Z.
             */
            if (contactAxis == Axis.Y)
            {
                float targetLeft =
                    targetMin.x;

                float targetRight =
                    targetMax.x;

                float targetBack =
                    targetMin.z;

                float targetFront =
                    targetMax.z;

                /*
                 * Snap the LEFT edge.
                 */
                if (Mathf.Abs(left - targetLeft) <=
                    EdgeSnapDistance)
                {
                    candidate.x +=
                        targetLeft - left;
                }
                /*
                 * Snap the RIGHT edge.
                 */
                else if (Mathf.Abs(right - targetRight) <=
                         EdgeSnapDistance)
                {
                    candidate.x +=
                        targetRight - right;
                }

                /*
                 * Recalculate after X correction.
                 */
                left =
                    candidate.x + newMin.x;

                right =
                    candidate.x + newMax.x;

                /*
                 * Snap BACK edge.
                 */
                if (Mathf.Abs(back - targetBack) <=
                    EdgeSnapDistance)
                {
                    candidate.z +=
                        targetBack - back;
                }
                /*
                 * Snap FRONT edge.
                 */
                else if (Mathf.Abs(front - targetFront) <=
                         EdgeSnapDistance)
                {
                    candidate.z +=
                        targetFront - front;
                }
            }

            /*
             * X contact means we can snap Y/Z.
             */
            else if (contactAxis == Axis.X)
            {
                if (Mathf.Abs(bottom - targetMin.y) <=
                    EdgeSnapDistance)
                {
                    candidate.y +=
                        targetMin.y - bottom;
                }
                else if (Mathf.Abs(top - targetMax.y) <=
                         EdgeSnapDistance)
                {
                    candidate.y +=
                        targetMax.y - top;
                }

                bottom =
                    candidate.y + newMin.y;

                top =
                    candidate.y + newMax.y;

                if (Mathf.Abs(back - targetMin.z) <=
                    EdgeSnapDistance)
                {
                    candidate.z +=
                        targetMin.z - back;
                }
                else if (Mathf.Abs(front - targetMax.z) <=
                         EdgeSnapDistance)
                {
                    candidate.z +=
                        targetMax.z - front;
                }
            }

            /*
             * Z contact means we can snap X/Y.
             */
            else
            {
                if (Mathf.Abs(left - targetMin.x) <=
                    EdgeSnapDistance)
                {
                    candidate.x +=
                        targetMin.x - left;
                }
                else if (Mathf.Abs(right - targetMax.x) <=
                         EdgeSnapDistance)
                {
                    candidate.x +=
                        targetMax.x - right;
                }

                left =
                    candidate.x + newMin.x;

                right =
                    candidate.x + newMax.x;

                if (Mathf.Abs(bottom - targetMin.y) <=
                    EdgeSnapDistance)
                {
                    candidate.y +=
                        targetMin.y - bottom;
                }
                else if (Mathf.Abs(top - targetMax.y) <=
                         EdgeSnapDistance)
                {
                    candidate.y +=
                        targetMax.y - top;
                }
            }

            return candidate;
        }

        // ============================================================
        // FALLBACK SURFACE PLACEMENT
        // ============================================================

        private bool TryGetSurfacePlacement(
            RaycastHit mouseHit,
            out Vector3 position,
            out Quaternion rotation)
        {
            position = Vector3.zero;

            rotation =
                selectedPrefab.transform.rotation;

            if (previewBoxCollider == null)
                return false;

            preview.transform.SetPositionAndRotation(
                Vector3.zero,
                rotation);

            Vector3[] corners =
                GetWorldBoxCorners(
                    previewBoxCollider);

            GetBounds(
                corners,
                out Vector3 min,
                out Vector3 max);

            Vector3 center =
                GetAverage(corners);

            Vector3 candidate =
                Vector3.zero;

            candidate.x =
                mouseHit.point.x -
                center.x;

            candidate.z =
                mouseHit.point.z -
                center.z;

            candidate.y =
                mouseHit.point.y -
                min.y;

            position = candidate;

            return true;
        }

        // ============================================================
        // BOX COLLIDER CORNERS
        // ============================================================

        private Vector3[] GetWorldBoxCorners(
            BoxCollider boxCollider)
        {
            Transform t =
                boxCollider.transform;

            Vector3 half =
                boxCollider.size * 0.5f;

            Vector3 center =
                boxCollider.center;

            return new[]
            {
                t.TransformPoint(
                    center + new Vector3(
                        -half.x,
                        -half.y,
                        -half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        half.x,
                        -half.y,
                        -half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        -half.x,
                        half.y,
                        -half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        half.x,
                        half.y,
                        -half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        -half.x,
                        -half.y,
                        half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        half.x,
                        -half.y,
                        half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        -half.x,
                        half.y,
                        half.z)),

                t.TransformPoint(
                    center + new Vector3(
                        half.x,
                        half.y,
                        half.z))
            };
        }

        private void GetBounds(
            Vector3[] points,
            out Vector3 min,
            out Vector3 max)
        {
            min = points[0];
            max = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                min = Vector3.Min(
                    min,
                    points[i]);

                max = Vector3.Max(
                    max,
                    points[i]);
            }
        }

        private Vector3 GetAverage(
            Vector3[] points)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < points.Length; i++)
            {
                result += points[i];
            }

            return result / points.Length;
        }

        private float GetExtremeCoordinate(
            Vector3[] points,
            Axis axis,
            bool maximum)
        {
            float value =
                GetAxisValue(
                    points[0],
                    axis);

            for (int i = 1; i < points.Length; i++)
            {
                float current =
                    GetAxisValue(
                        points[i],
                        axis);

                if (maximum)
                    value = Mathf.Max(
                        value,
                        current);
                else
                    value = Mathf.Min(
                        value,
                        current);
            }

            return value;
        }

        private float GetAxisValue(
            Vector3 value,
            Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return value.x;

                case Axis.Y:
                    return value.y;

                default:
                    return value.z;
            }
        }

        private enum Axis
        {
            X,
            Y,
            Z
        }

        // ============================================================
        // PLACE
        // ============================================================

        private void PlacePrefab()
        {
            if (selectedPrefab == null)
                return;

            if (preview == null)
                return;

            Vector3 position =
                preview.transform.position;

            Quaternion rotation =
                preview.transform.rotation;

            /*
             * Avoid creating duplicate objects when
             * dragging without moving enough.
             */
            if (hasLastPlacedPosition &&
                Vector3.Distance(
                    lastPlacedPosition,
                    position) <
                PlacementDistance)
            {
                return;
            }

            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(
                    selectedPrefab);

            if (instance == null)
                return;

            instance.transform.SetPositionAndRotation(
                position,
                rotation);

            if (placementRoot != null)
            {
                instance.transform.SetParent(
                    placementRoot.transform,
                    true);
            }

            Undo.RegisterCreatedObjectUndo(
                instance,
                "Place Level Object");

            Selection.activeGameObject =
                instance;

            lastPlacedPosition =
                position;

            hasLastPlacedPosition = true;
        }
    }
}