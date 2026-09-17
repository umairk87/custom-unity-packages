using UnityEngine;

namespace UKFramework.Game.Events
{
    public readonly struct LevelCompletedEvent
    {
        public readonly LevelCompleteData Data;

        public LevelCompletedEvent(LevelCompleteData _data)
        {
            Data = _data;
        }
    }

    public readonly struct LevelCompleteData
    {
        public readonly int Level;
        public readonly int Score;
        public readonly int Stars;

        public LevelCompleteData(
            int level,
            int score,
            int stars)
        {
            Level = level;
            Score = score;
            Stars = stars;
        }
    }


    public readonly struct LevelFailedData
    {
        public readonly int Level;


        public LevelFailedData(
            int level
          )
        {
            Level = level;

        }
    }
    public readonly struct MainMenuData
    {
    }



}