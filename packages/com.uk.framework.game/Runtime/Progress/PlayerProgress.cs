using System.Collections.Generic;
using UnityEngine;


namespace UKFramework.Game.Progress
{
    [System.Serializable]
    public class PlayerProgress
    {
        public int HighestUnlockedLevel;
        public int HighestCompletedLevel;
        public int Lives;
        public int Coins;
        public bool MusicEnabled = true;
        public bool SoundEnabled = true;
    }

}