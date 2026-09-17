
namespace UKFramework.Game
{
    public readonly struct NextLevelRequestedData
    {
    }
    public readonly struct RestartLevelRequestedData
    {
    }
    public readonly struct ContinueLevelRequestedData
    {
    }

    public readonly struct MainMenuRequestedData
    {
        public readonly bool isFromGameplay;

        public MainMenuRequestedData(bool val = false)
        {
            isFromGameplay = val;
        }
    }
    public readonly struct SubMenuRequestedData
    {
    }
    public readonly struct GamePlayPausedRequestedData
    {
    }
    public readonly struct GamePlayResumedRequestedData
    {
    }
}