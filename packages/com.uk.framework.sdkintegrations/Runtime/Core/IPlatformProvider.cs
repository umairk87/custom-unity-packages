namespace UK.Framework.SDKIntegrations
{
    public interface IPlatformProvider
    {
        ServiceProviderId Id { get; }
        bool IsAvailable { get; }

        void Initialize();
        void GameLoadingFinished();
        void GameplayStart();
        void GameplayStop();
    }
}
