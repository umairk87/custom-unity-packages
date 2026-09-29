using System.Collections.Generic;

namespace UK.Framework.SDKIntegrations
{
    public static class ServiceProviderFactory
    {
        public static IPlatformProvider CreatePlatform(ServiceProviderId id)
        {
            switch (id)
            {
                case ServiceProviderId.CrazyGames:
#if UK_SDK_CRAZYGAMES
                    return new Providers.CrazyGames.CrazyGamesPlatformProvider();
#else
                    return new Null.NullPlatformProvider();
#endif
                case ServiceProviderId.Poki:
#if UK_SDK_POKI
                    return new Providers.Poki.PokiPlatformProvider();
#else
                    return new Null.NullPlatformProvider();
#endif
                default:
                    return new Null.NullPlatformProvider();
            }
        }

        public static IAdsProvider CreateAds(ServiceProviderId id)
        {
            switch (id)
            {
                case ServiceProviderId.CrazyGames:
#if UK_SDK_CRAZYGAMES
                    return new Providers.CrazyGames.CrazyGamesAdsProvider();
#else
                    return new Null.NullAdsProvider();
#endif
                case ServiceProviderId.Poki:
#if UK_SDK_POKI
                    return new Providers.Poki.PokiAdsProvider();
#else
                    return new Null.NullAdsProvider();
#endif
                case ServiceProviderId.AdMob:
#if UK_SDK_ADMOB
                    return new Providers.AdMob.AdMobAdsProvider();
#else
                    return new Null.NullAdsProvider();
#endif
                case ServiceProviderId.UnityAds:
#if UK_SDK_UNITYADS
                    return new Providers.UnityAds.UnityAdsProvider();
#else
                    return new Null.NullAdsProvider();
#endif
                default:
                    return new Null.NullAdsProvider();
            }
        }

        public static IAnalyticsProvider CreateAnalytics(ServiceProviderId id)
        {
            switch (id)
            {
                case ServiceProviderId.CrazyGames:
#if UK_SDK_CRAZYGAMES
                    return new Providers.CrazyGames.CrazyGamesAnalyticsProvider();
#else
                    return new Null.NullAnalyticsProvider();
#endif
                case ServiceProviderId.Poki:
#if UK_SDK_POKI
                    return new Providers.Poki.PokiAnalyticsProvider();
#else
                    return new Null.NullAnalyticsProvider();
#endif
                case ServiceProviderId.Firebase:
#if UK_SDK_FIREBASE
                    return new Providers.Firebase.FirebaseAnalyticsProvider();
#else
                    return new Null.NullAnalyticsProvider();
#endif
                default:
                    return new Null.NullAnalyticsProvider();
            }
        }
    }
}
