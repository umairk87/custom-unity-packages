
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UKFramework.Game
{
    [DisallowMultipleComponent]
    public sealed class UIMapPanZoom : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IScrollHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;

        [Header("Zoom")]
        [SerializeField, Min(0.01f)] private float minZoom = 1f;
        [SerializeField, Min(0.01f)] private float maxZoom = 3f;
        [SerializeField, Min(0.0001f)] private float scrollSpeed = 0.01f;
        [SerializeField] private bool autoFitMinZoom = true;

        [Header("Options")]
        [SerializeField] private bool allowPan = true;
        [SerializeField] private bool allowZoom = true;

        private float zoom = 1f;
        private bool dragging;
        private bool pinching;

        private int dragPointerId = int.MinValue;

        private Vector2 dragStartPosition;
        private bool dragMoved;

        private const float DragThreshold = 10f;

        public float CurrentZoom => zoom;
        public bool IsDragging => dragging;
        public bool WasDragged => dragMoved;

        private void Awake()
        {
            if (viewport == null || content == null)
            {
                Debug.LogError(
                    "UIMapPanZoom: Missing references.", this);

                enabled = false;
                return;
            }

            zoom = content.localScale.x;

            SetZoom(zoom);
        }

        private void OnEnable()
        {
            dragging = false;
            pinching = false;
            dragMoved = false;
            dragPointerId = int.MinValue;
        }

        private void Update()
        {
            HandlePinch();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("MAP: Begin Drag");

            if (!allowPan || pinching || HasTwoTouches())
                return;

            dragging = true;
            dragPointerId = eventData.pointerId;
            dragStartPosition = eventData.position;
            dragMoved = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"MAP: Drag Delta = {eventData.delta}");

            if (!allowPan || !dragging ||
                pinching || HasTwoTouches() ||
                eventData.pointerId != dragPointerId)
                return;

            Camera cam = eventData.pressEventCamera;

            if (!ToViewport(
                eventData.position,
                cam,
                out Vector2 current))
                return;

            if (!ToViewport(
                eventData.position - eventData.delta,
                cam,
                out Vector2 previous))
                return;

            content.anchoredPosition += current - previous;

            Debug.Log(
                $"MAP: Position Before Clamp = {content.anchoredPosition}");

            ClampPosition();

            Debug.Log(
                $"MAP: Position After Clamp = {content.anchoredPosition}");
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != dragPointerId)
                return;

            dragging = false;
            dragPointerId = int.MinValue;
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!allowZoom || HasActiveTouches())
                return;

            float factor = Mathf.Exp(
                eventData.scrollDelta.y * scrollSpeed);

            ZoomAt(
                zoom * factor,
                eventData.position,
                eventData.enterEventCamera);
        }

        private void HandlePinch()
        {
            if (Touchscreen.current == null)
            {
                pinching = false;
                return;
            }

            var touches = Touchscreen.current.touches;

            int first = -1;
            int second = -1;

            for (int i = 0; i < touches.Count; i++)
            {
                if (!touches[i].press.isPressed)
                    continue;

                if (first < 0)
                    first = i;
                else
                {
                    second = i;
                    break;
                }
            }

            if (second < 0)
            {
                pinching = false;
                return;
            }

            var touchA = touches[first];
            var touchB = touches[second];

            Vector2 a = touchA.position.ReadValue();
            Vector2 b = touchB.position.ReadValue();

            Vector2 previousA =
                a - touchA.delta.ReadValue();

            Vector2 previousB =
                b - touchB.delta.ReadValue();

            Vector2 center = (a + b) * 0.5f;
            Vector2 previousCenter =
                (previousA + previousB) * 0.5f;

            Camera cam = GetCanvasCamera();

            // Only process gestures within the viewport.
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    viewport, center, cam))
            {
                pinching = false;
                return;
            }

            pinching = true;
            dragging = false;
            dragPointerId = int.MinValue;

            if (allowPan &&
                ToViewport(center, cam, out Vector2 current) &&
                ToViewport(previousCenter, cam, out Vector2 previous))
            {
                content.anchoredPosition += current - previous;
                ClampPosition();
            }

            if (!allowZoom)
                return;

            float oldDistance =
                Vector2.Distance(previousA, previousB);

            if (oldDistance <= 0.001f)
                return;

            float newDistance = Vector2.Distance(a, b);

            ZoomAt(
                zoom * newDistance / oldDistance,
                center,
                cam);
        }

        private void ZoomAt(
            float targetZoom,
            Vector2 screenPosition,
            Camera cam)
        {
            if (!allowZoom)
                return;

            targetZoom = ClampZoom(targetZoom);

            if (Mathf.Approximately(targetZoom, zoom))
                return;

            if (!RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    content,
                    screenPosition,
                    cam,
                    out Vector2 contentPoint))
                return;

            if (!ToViewport(
                    screenPosition,
                    cam,
                    out Vector2 viewportPoint))
                return;

            zoom = targetZoom;

            content.localScale = Vector3.one * zoom;

            content.anchoredPosition =
                viewportPoint - contentPoint * zoom;

            ClampPosition();
        }

        private float ClampZoom(float value)
        {
            float minimum = minZoom;

            if (autoFitMinZoom &&
                viewport != null &&
                content != null)
            {
                Vector2 viewSize = viewport.rect.size;
                Vector2 mapSize = content.rect.size;

                if (mapSize.x > 0f && mapSize.y > 0f)
                {
                    float fit = Mathf.Max(
                        viewSize.x / mapSize.x,
                        viewSize.y / mapSize.y);

                    minimum = Mathf.Max(minimum, fit);
                }
            }

            return Mathf.Clamp(
                value,
                minimum,
                Mathf.Max(minimum, maxZoom));
        }

        public void SetZoom(float value)
        {
            if (content == null)
                return;

            zoom = ClampZoom(value);

            content.localScale = Vector3.one * zoom;

            ClampPosition();
        }

        public void ZoomIn()
        {
            SetZoom(zoom * 1.25f);
        }

        public void ZoomOut()
        {
            SetZoom(zoom / 1.25f);
        }

        public void ResetView()
        {
            SetZoom(minZoom);

            content.anchoredPosition = Vector2.zero;

            ClampPosition();
        }

        public void FocusOn(RectTransform target)
        {
            if (target == null || content == null)
                return;

            Vector3 worldCenter =
                target.TransformPoint(target.rect.center);

            Vector3 localCenter =
                content.InverseTransformPoint(worldCenter);

            content.anchoredPosition =
                -(Vector2)localCenter * zoom;

            ClampPosition();
        }

        private void ClampPosition()
        {
            if (viewport == null || content == null)
                return;

            Vector2 viewSize = viewport.rect.size;

            Vector2 mapSize =
                Vector2.Scale(
                    content.rect.size,
                    new Vector2(zoom, zoom));

            Vector2 maxOffset = new Vector2(
                Mathf.Max(
                    0f,
                    (mapSize.x - viewSize.x) * 0.5f),

                Mathf.Max(
                    0f,
                    (mapSize.y - viewSize.y) * 0.5f));

            Vector2 position = content.anchoredPosition;

            position.x = Mathf.Clamp(
                position.x,
                -maxOffset.x,
                maxOffset.x);

            position.y = Mathf.Clamp(
                position.y,
                -maxOffset.y,
                maxOffset.y);

            content.anchoredPosition = position;
        }

        private bool ToViewport(
            Vector2 screenPosition,
            Camera cam,
            out Vector2 localPosition)
        {
            return RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    viewport,
                    screenPosition,
                    cam,
                    out localPosition);
        }

        private Camera GetCanvasCamera()
        {
            Canvas canvas =
                viewport.GetComponentInParent<Canvas>();

            if (canvas == null ||
                canvas.renderMode ==
                RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }

        private bool HasActiveTouches()
        {
            if (Touchscreen.current == null)
                return false;

            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                    return true;
            }

            return false;
        }

        private bool HasTwoTouches()
        {
            if (Touchscreen.current == null)
                return false;

            int count = 0;

            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    count++;

                    if (count >= 2)
                        return true;
                }
            }

            return false;
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled ||
                viewport == null ||
                content == null)
                return;

            SetZoom(zoom);
        }
    }
}
