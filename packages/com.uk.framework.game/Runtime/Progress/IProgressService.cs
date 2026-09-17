using System.Collections.Generic;
using UnityEngine;


namespace UKFramework.Game.Progress
{
    public interface IProgressService
    {
        int HighestUnlockedLevel { get; }
        int HighestCompletedLevel { get; }
        bool IsLevelUnlocked(int levelId);
        bool IsLevelCompleted(int levelId);
        void CompleteLevel(int levelId);
        int Lives { get; }
        int Coins { get; }
        void LoseLife();
        void AddLife(int amount);
        void AddCoins(int amount);
        bool MusicEnabled { get; }
        bool SoundEnabled { get; }

        void SetMusicEnabled(bool enabled);
        void SetSoundEnabled(bool enabled);
    }

}