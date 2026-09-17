using System.Collections.Generic;
using UKFramework.Game.Events;
using UKFramework.Game;
using UnityEngine;

namespace UKFramework.Game.UI
{
    public interface IUIService
    {
        void Initialize();
        void Show(UIScreenId id);
        void Show<T>(UIScreenId id, T data);
        void Hide(UIScreenId id);
        void HideAll();
        bool IsVisible(UIScreenId id);
    }


    public sealed class UIService : MonoBehaviour, IUIService, IGameInitializer
    {
        [SerializeField]
        private List<UIScreenEntry> screens =
            new();

        private readonly Dictionary<UIScreenId, UIScreen>
            lookup = new();
        private bool isInitialized;


        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            lookup.Clear();

            ServiceLocator.Register<IUIService>(this);
            foreach (var entry in screens)
            {
                if (entry.Screen == null)
                    continue;

                lookup[entry.Id] =
                    entry.Screen;

                entry.Screen.Initialize();
                entry.Screen.Hide();
            }
            isInitialized = true;

        }
        private void OnEnable()
        {
            // EventBus.Subscribe<LevelCompletedEvent>(
            //     OnLevelCompleted);

            // EventBus.Subscribe<LevelFailedEvent>(
            //     OnLevelFailed);
        }

        private void OnDisable()
        {
            // EventBus.Unsubscribe<LevelCompletedEvent>(
            //     OnLevelCompleted);

            // EventBus.Unsubscribe<LevelFailedEvent>(
            //     OnLevelFailed);
        }
        private void OnLevelCompleted(
            LevelCompletedEvent data)
        {
            Show(UIScreenId.LevelComplete);
        }

        private void OnLevelFailed(
            LevelFailedEvent data)
        {
            Show(UIScreenId.LevelFailed);
        }

        public void Show(UIScreenId id)
        {
            if (!lookup.TryGetValue(
                    id,
                    out UIScreen screen))
            {
                Debug.LogWarning(
                    $"UI not found: {id}");

                return;
            }

            screen.Show();
        }
        public void Show<T>(UIScreenId id, T data)
        {
            if (!lookup.TryGetValue(id, out UIScreen screen))
            {
                Debug.LogError($"UI Screen not found: {id}");
                return;
            }

            if (screen is UIScreen<T> typedScreen)
            {
                typedScreen.Show(data);
                return;
            }

            Debug.LogError(
                $"Screen {id} does not accept data of type {typeof(T).Name}");
        }


        public void Hide(UIScreenId id)
        {
            if (!lookup.TryGetValue(
                    id,
                    out UIScreen screen))
                return;

            screen.Hide();
        }


        public void HideAll()
        {
            foreach (var screen in lookup.Values)
                screen.Hide();
        }


        public bool IsVisible(UIScreenId id)
        {
            return lookup.TryGetValue(
                       id,
                       out UIScreen screen)
                   && screen.IsVisible;
        }
    }
}