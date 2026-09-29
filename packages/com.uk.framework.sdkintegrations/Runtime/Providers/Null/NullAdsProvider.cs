using System;

namespace UK.Framework.SDKIntegrations.Null
{
    public sealed class NullAdsProvider : IAdsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.None;
        public bool IsInitialized => false;

        public void Initialize(Action<bool> callback)
        {
            callback?.Invoke(false);
        }

        public bool ShowInterstitial(Action<AdsShowResult> callback)
        {
            return false;
        }

        public bool ShowRewarded(Action<AdsShowResult> callback)
        {
            return false;
        }

        public bool ShowBanner() => false;
        public void HideBanner() { }
    }
}
