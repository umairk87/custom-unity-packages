namespace UKFramework.Game
{
    public interface IGameStateService
    {
        GameState Current { get; }
        void SetState(GameState state);
    }

    public enum GameState
    {
        None,
        MainMenu,
        GameplayStarted,
        LevelFailed,
        LevelPassed,
        GameCompleted,
        GameplayPaused
    }
    public readonly struct GameStateChangedData
    {
        public readonly GameState Previous;
        public readonly GameState Current;

        public GameStateChangedData(
            GameState previous,
            GameState current)
        {
            Previous = previous;
            Current = current;
        }
    }

}