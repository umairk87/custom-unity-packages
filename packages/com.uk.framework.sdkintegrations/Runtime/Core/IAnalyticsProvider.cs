namespace UK.Framework.SDKIntegrations
{
    public interface IAnalyticsProvider
    {
        ServiceProviderId Id { get; }
        bool IsAvailable { get; }

        void Initialize();
        void TrackEvent(string eventName, string parameter = null, string value = null);
    }
}
