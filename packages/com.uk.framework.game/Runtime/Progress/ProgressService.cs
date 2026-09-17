using System.Collections.Generic;
using UnityEngine;
using UKFramework.Game.Events;


namespace UKFramework.Game.Progress
{
    using UnityEngine;

    public sealed class ProgressService : IProgressService
    {
        private const string SaveKey = "PLAYER_PROGRESS";

        private PlayerProgress progress;

        public int HighestUnlockedLevel =>
            progress.HighestUnlockedLevel;

        public int HighestCompletedLevel =>
            progress.HighestCompletedLevel;
        public int Lives => progress.Lives;
        public int Coins => progress.Coins;
        public bool MusicEnabled => progress.MusicEnabled;
        public bool SoundEnabled => progress.SoundEnabled;

        public ProgressService()
        {
            Load();
        }

        public bool IsLevelUnlocked(int levelId)
        {
            return levelId >= 0 &&
                   levelId <= progress.HighestUnlockedLevel;
        }

        public bool IsLevelCompleted(int levelId)
        {
            return levelId >= 0 &&
                   levelId <= progress.HighestCompletedLevel;
        }

        public void CompleteLevel(int levelId)
        {
            if (levelId < 0)
                return;

            // Already completed
            if (levelId <= progress.HighestCompletedLevel)
                return;

            progress.HighestCompletedLevel = levelId;

            // Unlock next level
            int nextLevel = levelId + 1;

            if (nextLevel > progress.HighestUnlockedLevel)
                progress.HighestUnlockedLevel = nextLevel;

            Save();
        }

        private void Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                CreateNewProgress();
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);

            if (string.IsNullOrEmpty(json))
            {
                CreateNewProgress();
                return;
            }

            progress = JsonUtility.FromJson<PlayerProgress>(json);

            if (progress == null)
                CreateNewProgress();
        }

        private void CreateNewProgress()
        {
            progress = new PlayerProgress
            {
                HighestUnlockedLevel = 0,
                HighestCompletedLevel = -1,
                Lives = 3,
                Coins = 0
            };
        }

        private void Save()
        {
            string json = JsonUtility.ToJson(progress);

            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void LoseLife()
        {
            if (progress.Lives <= 0)
                return;

            progress.Lives--;

            Save();

            EventBus.Publish(
                new LivesChangedData(progress.Lives));
        }

        public void AddLife(int amount)
        {
            if (amount <= 0)
                return;

            progress.Lives += amount;

            Save();

            EventBus.Publish(
                new LivesChangedData(progress.Lives));
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            progress.Coins += amount;

            Save();

            EventBus.Publish(
                new CoinsChangedData(progress.Coins));
        }


        #region Audio Progress
        public void SetMusicEnabled(bool enabled)
        {
            if (progress.MusicEnabled == enabled)
                return;

            progress.MusicEnabled = enabled;

            Save();

            EventBus.Publish(
                new MusicSettingChangedData(enabled));
        }

        public void SetSoundEnabled(bool enabled)
        {
            if (progress.SoundEnabled == enabled)
                return;

            progress.SoundEnabled = enabled;

            Save();

            EventBus.Publish(
                new SoundSettingChangedData(enabled));
        }
        #endregion

    }
}