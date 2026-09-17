using UnityEngine;

namespace UKFramework.Game.Events
{
    public readonly struct GamePlayStartedEvent
    {
        public readonly int Level;

        public GamePlayStartedEvent(int level)
        {
            Level = level;
        }
    }
}