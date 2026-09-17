using UnityEngine;

namespace UKFramework.Game.UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        public bool IsVisible =>
            gameObject.activeSelf;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject?.SetActive(false);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Deinitialize()
        {
        }
    }
    public abstract class UIScreen<T>
        : UIScreen
    {
        public virtual void Show(T data)
        {
            OnShow(data);

        }
        protected abstract void OnShow(T data);

    }
}