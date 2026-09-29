#if UK_SDK_CRAZYGAMES
using CrazyGames;

namespace UK.Framework.SDKIntegrations.Providers.CrazyGames
{
    public sealed class CrazyGamesAnalyticsProvider : IAnalyticsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.CrazyGames;
        public bool IsAvailable => CrazySDK.IsAvailable;

        public void Initialize()
        {
        }

        public void TrackEvent(
            string eventName,
            string parameter = null,
            string value = null)
        {
            // CrazyGames platform analytics are primarily driven by
            // SDK lifecycle/monetization events. Keep generic custom
            // analytics behind a separate analytics provider when needed.
        }
    }
}
#endif
