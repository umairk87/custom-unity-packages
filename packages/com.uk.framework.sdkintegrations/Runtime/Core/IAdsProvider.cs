using System;

namespace UK.Framework.SDKIntegrations
{
    public interface IAdsProvider
    {
        ServiceProviderId Id { get; }
        bool IsInitialized { get; }

        void Initialize(Action<bool> callback);

        // Return true only when the request was accepted by the provider.
        // The callback reports the final ad lifecycle result.
        bool ShowInterstitial(Action<AdsShowResult> callback);
        bool ShowRewarded(Action<AdsShowResult> callback);

        // Return true if the provider accepted the banner request.
        bool ShowBanner();
        void HideBanner();
    }
}
