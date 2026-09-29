namespace UK.Framework.SDKIntegrations
{
    public sealed class PlatformService
    {
        private readonly IPlatformProvider _provider;

        public bool IsAvailable => _provider != null && _provider.IsAvailable;
        public ServiceProviderId ProviderId => _provider?.Id ?? ServiceProviderId.None;

        public PlatformService(IPlatformProvider provider)
        {
            _provider = provider;
        }

        public void Initialize() => _provider?.Initialize();
        public void GameLoadingFinished() => _provider?.GameLoadingFinished();
        public void GameplayStart() => _provider?.GameplayStart();
        public void GameplayStop() => _provider?.GameplayStop();
    }
}
