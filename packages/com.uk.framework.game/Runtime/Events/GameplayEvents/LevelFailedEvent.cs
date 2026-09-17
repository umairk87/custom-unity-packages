using UnityEngine;

namespace UKFramework.Game.Events
{
    public readonly struct LevelFailedEvent
    {
        public readonly int Level;

        public LevelFailedEvent(int level)
        {
            Level = level;
        }
    }
}