#if UK_SDK_CRAZYGAMES
using System;
using CrazyGames;

namespace UK.Framework.SDKIntegrations.Providers.CrazyGames
{
    public sealed class CrazyGamesAdsProvider : IAdsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.CrazyGames;
        public bool IsInitialized { get; private set; }

        public void Initialize(Action<bool> callback)
        {
            IsInitialized = CrazySDK.IsAvailable;
            callback?.Invoke(IsInitialized);
        }

        public bool ShowInterstitial(Action<AdsShowResult> callback)
        {
            if (!IsInitialized)
                return false;

            CrazySDK.Ad.RequestAd(
                CrazyAdType.Midgame,
                () => callback?.Invoke(AdsShowResult.Started),
                _ => callback?.Invoke(AdsShowResult.Failed),
                () => callback?.Invoke(AdsShowResult.Completed));

            return true;
        }

        public bool ShowRewarded(Action<AdsShowResult> callback)
        {
            if (!IsInitialized)
                return false;

            CrazySDK.Ad.RequestAd(
                CrazyAdType.Rewarded,
                () => callback?.Invoke(AdsShowResult.Started),
                _ => callback?.Invoke(AdsShowResult.Failed),
                () => callback?.Invoke(AdsShowResult.Completed));

            return true;
        }

        public bool ShowBanner()
        {
            // CrazyGames banner API differs by SDK version/configuration.
            // Keep this adapter method isolated so only this file changes
            // if your installed SDK exposes a banner-specific API.
            return false;
        }

        public void HideBanner()
        {
        }
    }
}
#endif
