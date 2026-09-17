namespace UKFramework.Game.UI
{
    using System;
    using UnityEngine;


    [Serializable]
    public sealed class UIScreenEntry
    {
        public UIScreenId Id;
        public UIScreen Screen;
    }

    public enum UIScreenId
    {
        MainMenu,
        Gameplay,
        Pause,
        LevelComplete,
        LevelFailed,
        subMenu,
        Loading
    }
}