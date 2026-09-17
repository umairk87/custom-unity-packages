namespace UKFramework.Game
{
    public readonly struct BallsChangedData
    {
        public readonly int Previous;
        public readonly int Current;
        public readonly int Delta;

        public BallsChangedData(
            int previous,
            int current)
        {
            Previous = previous;
            Current = current;
            Delta = current - previous;
        }
    }

    public readonly struct BallFinishedData
    {
        public readonly bool isFinished;
        public BallFinishedData(bool _isFinished)
        {
            isFinished = _isFinished;
        }
    }

    public readonly struct LivesChangedData
    {
        public readonly int Lives;

        public LivesChangedData(int lives)
        {
            Lives = lives;
        }
    }
    public readonly struct CoinsChangedData
    {
        public readonly int Coins;

        public CoinsChangedData(int coins)
        {
            Coins = coins;
        }
    }
    public readonly struct MusicSettingChangedData
    {
        public readonly bool Enabled;

        public MusicSettingChangedData(bool enabled)
        {
            Enabled = enabled;
        }
    }

    public readonly struct SoundSettingChangedData
    {
        public readonly bool Enabled;

        public SoundSettingChangedData(bool enabled)
        {
            Enabled = enabled;
        }
    }
    public readonly struct CoinsAnimationData
    {

    }

}