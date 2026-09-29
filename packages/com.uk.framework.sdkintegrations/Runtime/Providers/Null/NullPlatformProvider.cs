namespace UK.Framework.SDKIntegrations.Null
{
    public sealed class NullPlatformProvider : IPlatformProvider
    {
        public ServiceProviderId Id => ServiceProviderId.None;
        public bool IsAvailable => false;

        public void Initialize() { }
        public void GameLoadingFinished() { }
        public void GameplayStart() { }
        public void GameplayStop() { }
    }
}
