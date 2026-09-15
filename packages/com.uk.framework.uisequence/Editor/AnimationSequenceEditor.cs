using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Framework.UISequence;

namespace Framework.UISequence.UIEditor
{
    [CustomEditor(typeof(AnimationSequence))]
    public class AnimationSequenceEditor : Editor
    {
        private ReorderableList actionList;
        private SerializedProperty actionsProperty;

        private const float ElementTopPadding = 4f;
        private const float ElementBottomPadding = 6f;
        private const float HeaderHeight = 20f;
        private const float PropertySpacing = 3f;

        // =========================================================
        // ENABLE
        // =========================================================

        private void OnEnable()
        {
            actionsProperty =
                serializedObject.FindProperty("actions");

            if (actionsProperty == null)
                return;

            actionList = new ReorderableList(
                serializedObject,
                actionsProperty,
                true,
                true,
                false,
                false);

            actionList.drawHeaderCallback =
                DrawHeader;

            actionList.drawElementCallback =
                DrawElement;

            actionList.elementHeightCallback =
                GetElementHeight;

            actionList.drawElementBackgroundCallback =
                DrawElementBackground;

            actionList.onReorderCallback =
                OnReorder;
        }

        // =========================================================
        // INSPECTOR
        // =========================================================

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPlaybackButtons();

            EditorGUILayout.Space(8f);

            if (actionList != null)
            {
                actionList.DoLayoutList();
            }

            EditorGUILayout.Space(5f);

            if (GUILayout.Button(
                    "+ Add Action",
                    GUILayout.Height(30f)))
            {
                ShowAddActionMenu();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // =========================================================
        // PLAYBACK
        // =========================================================

        private void DrawPlaybackButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "▶ Play",
                    GUILayout.Height(26f)))
            {
                AnimationSequence sequence =
                    (AnimationSequence)target;

                sequence.Play();
            }

            if (GUILayout.Button(
                    "■ Stop",
                    GUILayout.Height(26f)))
            {
                AnimationSequence sequence =
                    (AnimationSequence)target;

                sequence.Stop();
            }

            if (GUILayout.Button(
                    "↻ Restart",
                    GUILayout.Height(26f)))
            {
                AnimationSequence sequence =
                    (AnimationSequence)target;

                sequence.Restart();
            }

            EditorGUILayout.EndHorizontal();
        }

        // =========================================================
        // HEADER
        // =========================================================

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(
                rect,
                "Animation Actions",
                EditorStyles.boldLabel);
        }

        // =========================================================
        // ELEMENT
        // =========================================================

        private void DrawElement(
            Rect rect,
            int index,
            bool active,
            bool focused)
        {
            if (index < 0 ||
                index >= actionsProperty.arraySize)
            {
                return;
            }

            SerializedProperty action =
                actionsProperty.GetArrayElementAtIndex(index);

            SerializedProperty typeProperty =
                action.FindPropertyRelative("type");

            if (typeProperty == null)
                return;

            AnimationActionType type =
                (AnimationActionType)
                typeProperty.enumValueIndex;

            // -----------------------------------------------------
            // Header
            // -----------------------------------------------------

            Rect headerRect = new Rect(
                rect.x + 4f,
                rect.y + ElementTopPadding,
                rect.width - 8f,
                HeaderHeight);

            // Leave room for ReorderableList drag handle.
            float titleX =
                headerRect.x + 18f;

            Rect titleRect = new Rect(
                titleX,
                headerRect.y,
                headerRect.width - 42f,
                HeaderHeight);

            EditorGUI.LabelField(
                titleRect,
                $"{index}  {type}",
                EditorStyles.boldLabel);

            // -----------------------------------------------------
            // Delete
            // -----------------------------------------------------

            Rect deleteRect = new Rect(
                headerRect.xMax - 20f,
                headerRect.y,
                20f,
                18f);

            if (GUI.Button(
                    deleteRect,
                    "×",
                    EditorStyles.miniButton))
            {
                actionsProperty.DeleteArrayElementAtIndex(
                    index);

                serializedObject.ApplyModifiedProperties();

                GUIUtility.ExitGUI();
            }

            // -----------------------------------------------------
            // Body
            // -----------------------------------------------------

            Rect bodyRect = new Rect(
                rect.x + 4f,
                rect.y +
                ElementTopPadding +
                HeaderHeight +
                3f,
                rect.width - 8f,
                GetBodyHeight(action));

            DrawActionBody(
                bodyRect,
                action);
        }

        // =========================================================
        // BACKGROUND
        // =========================================================

        private void DrawElementBackground(
            Rect rect,
            int index,
            bool active,
            bool focused)
        {
            if (!active)
                return;

            Color background =
                EditorGUIUtility.isProSkin
                    ? new Color(
                        0.22f,
                        0.22f,
                        0.22f)
                    : new Color(
                        0.82f,
                        0.82f,
                        0.82f);

            EditorGUI.DrawRect(
                new Rect(
                    rect.x + 1f,
                    rect.y + 1f,
                    rect.width - 2f,
                    rect.height - 2f),
                background);
        }

        // =========================================================
        // ELEMENT HEIGHT
        // =========================================================

        private float GetElementHeight(int index)
        {
            if (index < 0 ||
                index >= actionsProperty.arraySize)
            {
                return 40f;
            }

            SerializedProperty action =
                actionsProperty.GetArrayElementAtIndex(index);

            return ElementTopPadding
                   + HeaderHeight
                   + 3f
                   + GetBodyHeight(action)
                   + ElementBottomPadding;
        }

        // =========================================================
        // BODY HEIGHT
        // =========================================================

        private float GetBodyHeight(
            SerializedProperty action)
        {
            SerializedProperty typeProperty =
                action.FindPropertyRelative("type");

            if (typeProperty == null)
                return 25f;

            AnimationActionType type =
                (AnimationActionType)
                typeProperty.enumValueIndex;

            float height = 4f;

            switch (type)
            {
                case AnimationActionType.Delay:

                    height +=
                        PropertyHeight(
                            action,
                            "delay");

                    break;

                case AnimationActionType.Enable:
                case AnimationActionType.Disable:

                    height +=
                        PropertyHeight(
                            action,
                            "objectTarget");

                    break;

                case AnimationActionType.Move:

                    height +=
                        GetMoveHeight(action);

                    break;

                case AnimationActionType.Scale:

                    height +=
                        GetScaleHeight(action);

                    break;

                case AnimationActionType.Rotate:

                    height +=
                        GetRotateHeight(action);

                    break;

                case AnimationActionType.Fade:

                    height +=
                        PropertyHeight(
                            action,
                            "canvasGroup");

                    height +=
                        PropertyHeight(
                            action,
                            "alpha");

                    height +=
                        PropertyHeight(
                            action,
                            "fadeDuration");

                    height +=
                        PropertyHeight(
                            action,
                            "fadeEase");

                    break;

                case AnimationActionType.Sound:

                    height +=
                        PropertyHeight(
                            action,
                            "sound");

                    break;

                case AnimationActionType.Callback:

                    height +=
                        PropertyHeight(
                            action,
                            "callback");

                    break;
            }

            return height;
        }

        // =========================================================
        // MOVE HEIGHT
        // =========================================================

        private float GetMoveHeight(
            SerializedProperty action)
        {
            float height = 0f;

            height += PropertyHeight(
                action,
                "moveTarget");

            height += PropertyHeight(
                action,
                "startPoint");

            height += PropertyHeight(
                action,
                "endPoint");

            height += PropertyHeight(
                action,
                "moveTween");

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "moveTween");

            if (tween == null)
                return height;

            MoveTweenType tweenType =
                (MoveTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case MoveTweenType.Move:

                    height += PropertyHeight(
                        action,
                        "moveDuration");

                    height += PropertyHeight(
                        action,
                        "moveEase");

                    break;

                case MoveTweenType.Punch:

                    height += PropertyHeight(
                        action,
                        "punch");

                    height += PropertyHeight(
                        action,
                        "punchDuration");

                    height += PropertyHeight(
                        action,
                        "punchVibrato");

                    height += PropertyHeight(
                        action,
                        "punchElasticity");

                    break;

                case MoveTweenType.Shake:

                    height += PropertyHeight(
                        action,
                        "shakeStrength");

                    height += PropertyHeight(
                        action,
                        "shakeDuration");

                    height += PropertyHeight(
                        action,
                        "shakeVibrato");

                    height += PropertyHeight(
                        action,
                        "shakeRandomness");

                    break;

                case MoveTweenType.Jump:

                    height += PropertyHeight(
                        action,
                        "jumpPower");

                    height += PropertyHeight(
                        action,
                        "jumpCount");

                    height += PropertyHeight(
                        action,
                        "moveDuration");

                    height += PropertyHeight(
                        action,
                        "moveEase");

                    break;
            }

            return height;
        }

        // =========================================================
        // SCALE HEIGHT
        // =========================================================

        private float GetScaleHeight(
            SerializedProperty action)
        {
            float height = 0f;

            height += PropertyHeight(
                action,
                "transformTarget");

            height += PropertyHeight(
                action,
                "scaleTween");

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "scaleTween");

            if (tween == null)
                return height;

            ScaleTweenType tweenType =
                (ScaleTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case ScaleTweenType.Scale:

                    height += PropertyHeight(
                        action,
                        "scale");

                    height += PropertyHeight(
                        action,
                        "scaleDuration");

                    height += PropertyHeight(
                        action,
                        "scaleEase");

                    break;

                case ScaleTweenType.Punch:

                    height += PropertyHeight(
                        action,
                        "punch");

                    height += PropertyHeight(
                        action,
                        "punchDuration");

                    height += PropertyHeight(
                        action,
                        "punchVibrato");

                    height += PropertyHeight(
                        action,
                        "punchElasticity");

                    break;

                case ScaleTweenType.Shake:

                    height += PropertyHeight(
                        action,
                        "shakeStrength");

                    height += PropertyHeight(
                        action,
                        "shakeDuration");

                    height += PropertyHeight(
                        action,
                        "shakeVibrato");

                    height += PropertyHeight(
                        action,
                        "shakeRandomness");

                    break;
            }

            return height;
        }

        // =========================================================
        // ROTATE HEIGHT
        // =========================================================

        private float GetRotateHeight(
            SerializedProperty action)
        {
            float height = 0f;

            height += PropertyHeight(
                action,
                "transformTarget");

            height += PropertyHeight(
                action,
                "rotateTween");

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "rotateTween");

            if (tween == null)
                return height;

            RotateTweenType tweenType =
                (RotateTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case RotateTweenType.Rotate:

                    height += PropertyHeight(
                        action,
                        "rotation");

                    height += PropertyHeight(
                        action,
                        "rotateDuration");

                    height += PropertyHeight(
                        action,
                        "rotateEase");

                    break;

                case RotateTweenType.Punch:

                    height += PropertyHeight(
                        action,
                        "punch");

                    height += PropertyHeight(
                        action,
                        "punchDuration");

                    height += PropertyHeight(
                        action,
                        "punchVibrato");

                    height += PropertyHeight(
                        action,
                        "punchElasticity");

                    break;

                case RotateTweenType.Shake:

                    height += PropertyHeight(
                        action,
                        "shakeStrength");

                    height += PropertyHeight(
                        action,
                        "shakeDuration");

                    height += PropertyHeight(
                        action,
                        "shakeVibrato");

                    height += PropertyHeight(
                        action,
                        "shakeRandomness");

                    break;
            }

            return height;
        }

        // =========================================================
        // PROPERTY HEIGHT
        // =========================================================

        private float PropertyHeight(
            SerializedProperty parent,
            string propertyName)
        {
            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            return PropertyHeight(property);
        }

        private float PropertyHeight(
            SerializedProperty property)
        {
            if (property == null)
                return 0f;

            return EditorGUI.GetPropertyHeight(
                       property,
                       true)
                   + PropertySpacing;
        }

        // =========================================================
        // BODY DRAW
        // =========================================================

        private void DrawActionBody(
            Rect rect,
            SerializedProperty action)
        {
            SerializedProperty typeProperty =
                action.FindPropertyRelative("type");

            if (typeProperty == null)
                return;

            AnimationActionType type =
                (AnimationActionType)
                typeProperty.enumValueIndex;

            float y = rect.y;

            switch (type)
            {
                case AnimationActionType.Delay:

                    DrawProperty(
                        action,
                        "delay",
                        "Delay",
                        ref y,
                        rect);

                    break;

                case AnimationActionType.Enable:

                case AnimationActionType.Disable:

                    DrawProperty(
                        action,
                        "objectTarget",
                        "Target",
                        ref y,
                        rect);

                    break;

                case AnimationActionType.Move:

                    DrawMove(
                        rect,
                        action,
                        ref y);

                    break;

                case AnimationActionType.Scale:

                    DrawScale(
                        rect,
                        action,
                        ref y);

                    break;

                case AnimationActionType.Rotate:

                    DrawRotate(
                        rect,
                        action,
                        ref y);

                    break;

                case AnimationActionType.Fade:

                    DrawProperty(
                        action,
                        "canvasGroup",
                        "Canvas Group",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "alpha",
                        "Alpha",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "fadeDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "fadeEase",
                        "Ease",
                        ref y,
                        rect);

                    break;

                case AnimationActionType.Sound:

                    DrawProperty(
                        action,
                        "audioSource",
                        "audioClip",
                        ref y,
                        rect);

                    break;

                case AnimationActionType.Callback:

                    DrawProperty(
                        action,
                        "callback",
                        "Callback",
                        ref y,
                        rect);

                    break;
            }
        }

        // =========================================================
        // MOVE
        // =========================================================

        private void DrawMove(
            Rect rect,
            SerializedProperty action,
            ref float y)
        {
            DrawProperty(
                action,
                "moveTarget",
                "Target",
                ref y,
                rect);

            DrawProperty(
                action,
                "startPoint",
                "Start Point",
                ref y,
                rect);

            DrawProperty(
                action,
                "endPoint",
                "End Point",
                ref y,
                rect);

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "moveTween");

            DrawProperty(
                tween,
                "Tween",
                ref y,
                rect);

            if (tween == null)
                return;

            MoveTweenType tweenType =
                (MoveTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case MoveTweenType.Move:

                    DrawProperty(
                        action,
                        "moveDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "moveEase",
                        "Ease",
                        ref y,
                        rect);

                    break;

                case MoveTweenType.Punch:

                    DrawProperty(
                        action,
                        "punch",
                        "Punch",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchElasticity",
                        "Elasticity",
                        ref y,
                        rect);

                    break;

                case MoveTweenType.Shake:

                    DrawProperty(
                        action,
                        "shakeStrength",
                        "Strength",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeRandomness",
                        "Randomness",
                        ref y,
                        rect);

                    break;

                case MoveTweenType.Jump:

                    DrawProperty(
                        action,
                        "jumpPower",
                        "Jump Power",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "jumpCount",
                        "Jump Count",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "moveDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "moveEase",
                        "Ease",
                        ref y,
                        rect);

                    break;
            }
        }

        // =========================================================
        // SCALE
        // =========================================================

        private void DrawScale(
            Rect rect,
            SerializedProperty action,
            ref float y)
        {
            DrawProperty(
                action,
                "transformTarget",
                "Target",
                ref y,
                rect);

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "scaleTween");

            DrawProperty(
                tween,
                "Tween",
                ref y,
                rect);

            if (tween == null)
                return;

            ScaleTweenType tweenType =
                (ScaleTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case ScaleTweenType.Scale:

                    DrawProperty(
                        action,
                        "scale",
                        "Scale",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "scaleDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "scaleEase",
                        "Ease",
                        ref y,
                        rect);

                    break;

                case ScaleTweenType.Punch:

                    DrawProperty(
                        action,
                        "punch",
                        "Punch",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchElasticity",
                        "Elasticity",
                        ref y,
                        rect);

                    break;

                case ScaleTweenType.Shake:

                    DrawProperty(
                        action,
                        "shakeStrength",
                        "Strength",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeRandomness",
                        "Randomness",
                        ref y,
                        rect);

                    break;
            }
        }

        // =========================================================
        // ROTATE
        // =========================================================

        private void DrawRotate(
            Rect rect,
            SerializedProperty action,
            ref float y)
        {
            DrawProperty(
                action,
                "transformTarget",
                "Target",
                ref y,
                rect);

            SerializedProperty tween =
                action.FindPropertyRelative(
                    "rotateTween");

            DrawProperty(
                tween,
                "Tween",
                ref y,
                rect);

            if (tween == null)
                return;

            RotateTweenType tweenType =
                (RotateTweenType)
                tween.enumValueIndex;

            switch (tweenType)
            {
                case RotateTweenType.Rotate:

                    DrawProperty(
                        action,
                        "rotation",
                        "Rotation",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "rotateDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "rotateEase",
                        "Ease",
                        ref y,
                        rect);

                    break;

                case RotateTweenType.Punch:

                    DrawProperty(
                        action,
                        "punch",
                        "Punch",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "punchElasticity",
                        "Elasticity",
                        ref y,
                        rect);

                    break;

                case RotateTweenType.Shake:

                    DrawProperty(
                        action,
                        "shakeStrength",
                        "Strength",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeDuration",
                        "Duration",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeVibrato",
                        "Vibrato",
                        ref y,
                        rect);

                    DrawProperty(
                        action,
                        "shakeRandomness",
                        "Randomness",
                        ref y,
                        rect);

                    break;
            }
        }

        // =========================================================
        // PROPERTY DRAW
        // =========================================================

        private void DrawProperty(
            SerializedProperty parent,
            string propertyName,
            string label,
            ref float y,
            Rect container)
        {
            if (parent == null)
                return;

            SerializedProperty property =
                parent.FindPropertyRelative(
                    propertyName);

            DrawProperty(
                property,
                label,
                ref y,
                container);
        }

        private void DrawProperty(
            SerializedProperty property,
            string label,
            ref float y,
            Rect container)
        {
            if (property == null)
                return;

            float height =
                EditorGUI.GetPropertyHeight(
                    property,
                    true);

            Rect propertyRect =
                new Rect(
                    container.x,
                    y,
                    container.width,
                    height);

            EditorGUI.PropertyField(
                propertyRect,
                property,
                new GUIContent(label),
                true);

            y += height + PropertySpacing;
        }

        // =========================================================
        // ADD ACTION
        // =========================================================

        private void ShowAddActionMenu()
        {
            GenericMenu menu =
                new GenericMenu();

            AddMenuItem(
                menu,
                "Delay",
                AnimationActionType.Delay);

            AddMenuItem(
                menu,
                "Enable",
                AnimationActionType.Enable);

            AddMenuItem(
                menu,
                "Disable",
                AnimationActionType.Disable);

            menu.AddSeparator("");

            AddMenuItem(
                menu,
                "Move",
                AnimationActionType.Move);

            AddMenuItem(
                menu,
                "Scale",
                AnimationActionType.Scale);

            AddMenuItem(
                menu,
                "Rotate",
                AnimationActionType.Rotate);

            AddMenuItem(
                menu,
                "Fade",
                AnimationActionType.Fade);

            menu.AddSeparator("");

            AddMenuItem(
                menu,
                "Sound",
                AnimationActionType.Sound);

            AddMenuItem(
                menu,
                "Callback",
                AnimationActionType.Callback);

            menu.ShowAsContext();
        }

        private void AddMenuItem(
            GenericMenu menu,
            string name,
            AnimationActionType type)
        {
            menu.AddItem(
                new GUIContent(name),
                false,
                () => AddAction(type));
        }

        // =========================================================
        // ADD
        // =========================================================

        private void AddAction(
            AnimationActionType type)
        {
            int index =
                actionsProperty.arraySize;

            actionsProperty.InsertArrayElementAtIndex(
                index);

            SerializedProperty action =
                actionsProperty.GetArrayElementAtIndex(
                    index);

            SerializedProperty typeProperty =
                action.FindPropertyRelative(
                    "type");

            if (typeProperty != null)
            {
                typeProperty.enumValueIndex =
                    (int)type;
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        // =========================================================
        // REORDER
        // =========================================================

        private void OnReorder(
            ReorderableList list)
        {
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);

            Repaint();
        }
    }
}