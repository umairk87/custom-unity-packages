namespace UK.Framework.SDKIntegrations.Null
{
    public sealed class NullAnalyticsProvider : IAnalyticsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.None;
        public bool IsAvailable => false;

        public void Initialize() { }

        public void TrackEvent(
            string eventName,
            string parameter = null,
            string value = null)
        {
        }
    }
}
