#if UK_SDK_POKI

namespace UK.Framework.SDKIntegrations.Providers.Poki
{
    public sealed class PokiAnalyticsProvider : IAnalyticsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.Poki;
        public bool IsAvailable => PokiUnitySDK.Instance.isInitialized();

        public void Initialize()
        {
        }

        public void TrackEvent(
            string eventName,
            string parameter = null,
            string value = null)
        {
            PokiUnitySDK.Instance.measure(
                eventName,
                parameter ?? string.Empty,
                value ?? string.Empty);
        }
    }
}
#endif
