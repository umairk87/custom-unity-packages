#if UK_SDK_ADMOB

// Adapter boundary for Google Mobile Ads.
// Keep all Google Mobile Ads API calls inside this file.
// The exact API depends on the Google Mobile Ads Unity SDK version
// installed in the project.

using System;

namespace UK.Framework.SDKIntegrations.Providers.AdMob
{
    public sealed class AdMobAdsProvider : IAdsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.AdMob;
        public bool IsInitialized { get; private set; }

        public void Initialize(Action<bool> callback)
        {
            // Initialize your installed Google Mobile Ads SDK here.
            // Set IsInitialized=true only after initialization succeeds.
            IsInitialized = false;
            callback?.Invoke(false);
        }

        public bool ShowInterstitial(Action<AdsShowResult> callback) => false;
        public bool ShowRewarded(Action<AdsShowResult> callback) => false;
        public bool ShowBanner() => false;
        public void HideBanner() { }
    }
}
#endif
