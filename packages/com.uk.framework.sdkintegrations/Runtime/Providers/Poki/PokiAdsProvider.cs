#if UK_SDK_POKI
using System;

namespace UK.Framework.SDKIntegrations.Providers.Poki
{
    public sealed class PokiAdsProvider : IAdsProvider
    {
        public ServiceProviderId Id => ServiceProviderId.Poki;
        public bool IsInitialized => PokiUnitySDK.Instance.isInitialized();

        public void Initialize(Action<bool> callback)
        {
            callback?.Invoke(IsInitialized);
        }

        public bool ShowInterstitial(Action<AdsShowResult> callback)
        {
            if (!IsInitialized)
                return false;

            PokiUnitySDK.Instance.commercialBreakCallBack = () =>
            {
                callback?.Invoke(AdsShowResult.Completed);
            };

            PokiUnitySDK.Instance.commercialBreak();
            return true;
        }

        public bool ShowRewarded(Action<AdsShowResult> callback)
        {
            if (!IsInitialized)
                return false;

            PokiUnitySDK.Instance.rewardedBreakCallBack = success =>
            {
                callback?.Invoke(
                    success
                        ? AdsShowResult.Completed
                        : AdsShowResult.Failed);
            };

            PokiUnitySDK.Instance.rewardedBreak();
            return true;
        }

        public bool ShowBanner()
        {
            // Poki's primary ad flow is commercial/rewarded breaks.
            return false;
        }

        public void HideBanner()
        {
        }
    }
}
#endif
