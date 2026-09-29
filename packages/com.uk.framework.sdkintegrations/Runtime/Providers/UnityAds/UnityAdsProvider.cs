#if UK_SDK_UNITYADS

// Adapter boundary for Unity Ads / LevelPlay / the SDK version you use.
// Keep all vendor-specific API calls inside this file.

using System;

namespace UK.Framework.SDKIntegrations.Providers.UnityAds
{
    public sealed class UnityAdsProvider : IAdsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.UnityAds;
        public bool IsInitialized { get; private set; }

        public void Initialize(Action<bool> callback)
        {
            // Initialize your installed Unity Ads SDK here.
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
