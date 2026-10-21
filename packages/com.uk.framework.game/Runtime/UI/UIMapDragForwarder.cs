
using UnityEngine;
using UnityEngine.EventSystems;

namespace UKFramework.Game
{
    [DisallowMultipleComponent]
    public sealed class UIMapDragForwarder : MonoBehaviour,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private UIMapPanZoom map;

        private void Awake()
        {
            if (map == null)
                map = GetComponentInParent<UIMapPanZoom>();
        }

        public void OnInitializePotentialDrag(
            PointerEventData eventData)
        {
            eventData.useDragThreshold = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            map?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            map?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            map?.OnEndDrag(eventData);
        }
    }
}
