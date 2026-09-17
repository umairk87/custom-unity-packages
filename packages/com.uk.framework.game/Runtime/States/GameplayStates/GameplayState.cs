using UKFramework.Game.Events;
namespace UKFramework.Game
{

    public interface IGameplayState
    {
        int Score { get; }
        int Balls { get; }
        int Lives { get; }
    }

    public sealed class GameplayState : IGameplayState
    {
        public int Score { get; private set; }
        public int Balls { get; private set; }
        public int Lives { get; private set; }

        public GameplayState(
            int initialBalls,
            int initialLives)
        {
            Balls = initialBalls;
            Lives = initialLives;
        }

        public void AddScore(int amount)
        {
            Score += amount;

            // EventBus.Publish(
            //     new ScoreChangedData(
            //         Score - amount,
            //         Score));
        }
        public void AddBalls(int amount)
        {
            int previous = Balls;
            Balls += amount;
            EventBus.Publish(
               new BallsChangedData(
                   previous,
                   Balls));

        }

        public void UseBall()
        {
            if (Balls <= 0)
                return;

            int previous = Balls;

            Balls--;

            EventBus.Publish(
                new BallsChangedData(
                    previous,
                    Balls));
        }

        public void LoseLife()
        {
            if (Lives <= 0)
                return;
            Lives--;
            EventBus.Publish(
                new LivesChangedData(
                    Lives));
        }
        public void SetInitialState(int _balls, int _lives)
        {
            Balls = _balls;
            Lives = _lives;

        }
    }
}