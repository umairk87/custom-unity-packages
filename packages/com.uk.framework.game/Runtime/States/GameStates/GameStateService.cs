
namespace UKFramework.Game
{
    public sealed class GameStateService : IGameStateService
    {
        public GameState Current { get; private set; }

        public GameStateService()
        {
            Current = GameState.None;
        }

        public void SetState(GameState state)
        {
            if (Current == state)
                return;

            GameState previous = Current;

            Current = state;

            // EventBus.Publish(
            //     new GameStateChangedData(
            //         previous,
            //         Current));
        }
    }

}