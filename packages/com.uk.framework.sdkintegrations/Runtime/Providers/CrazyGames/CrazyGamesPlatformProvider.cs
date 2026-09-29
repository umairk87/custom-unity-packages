#if UK_SDK_CRAZYGAMES
using CrazyGames;

namespace UK.Framework.SDKIntegrations.Providers.CrazyGames
{
    public sealed class CrazyGamesPlatformProvider : IPlatformProvider
    {
        public ServiceProviderId Id => ServiceProviderId.CrazyGames;
        public bool IsAvailable => CrazySDK.IsAvailable;

        public void Initialize()
        {
            if (CrazySDK.IsAvailable)
                CrazySDK.Init();
        }

        public void GameLoadingFinished()
        {
            // Unity loading is handled by the Unity loader according to
            // CrazyGames documentation; no loading event is required here.
        }

        public void GameplayStart()
        {
            if (CrazySDK.IsAvailable)
                CrazySDK.Game.GameplayStart();
        }

        public void GameplayStop()
        {
            if (CrazySDK.IsAvailable)
                CrazySDK.Game.GameplayStop();
        }
    }
}
#endif
