#if UK_SDK_FIREBASE

using UnityEngine;

namespace UK.Framework.SDKIntegrations.Providers.Firebase
{
    public sealed class FirebaseAnalyticsProvider : IAnalyticsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.Firebase;
        public bool IsAvailable => true;

        public void Initialize()
        {
        }

        public void TrackEvent(
            string eventName,
            string parameter = null,
            string value = null)
        {
            // Keep Firebase-specific calls isolated here.
            // Example with Firebase Analytics:
            // Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //     eventName,
            //     parameter,
            //     value);
        }
    }
}
#endif
