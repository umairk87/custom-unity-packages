#if UK_SDK_POKI

namespace UK.Framework.SDKIntegrations.Providers.Poki
{
    public sealed class PokiPlatformProvider : IPlatformProvider
    {
        public ServiceProviderId Id => ServiceProviderId.Poki;
        public bool IsAvailable => PokiUnitySDK.Instance.isInitialized();

        public void Initialize()
        {
            PokiUnitySDK.Instance.init();
        }

        public void GameLoadingFinished()
        {
            PokiUnitySDK.Instance.gameLoadingFinished();
        }

        public void GameplayStart()
        {
            PokiUnitySDK.Instance.gameplayStart();
        }

        public void GameplayStop()
        {
            PokiUnitySDK.Instance.gameplayStop();
        }
    }
}
#endif
