using System.Collections.Generic;
using UnityEngine;

namespace UK.Framework.SDKIntegrations
{
    public sealed class SDKIntegrationsBootstrap : MonoBehaviour
    {
        [SerializeField] private SDKIntegrationsConfig _config;
        [SerializeField] private string _profileId = "mobile";

        public PlatformService Platform { get; private set; }
        public AdsService Ads { get; private set; }
        public AnalyticsService Analytics { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SDKIntegrationsRuntime.Register(this);
            Initialize(_profileId);
        }

        public void Initialize(string profileId)
        {
            if (_config == null)
            {
                Debug.LogError("[SDKIntegrations] SDKIntegrationsConfig is missing.");
                return;
            }

            var profile = _config.GetProfile(profileId);

            if (profile == null)
            {
                Debug.LogError($"[SDKIntegrations] Profile '{profileId}' was not found.");
                return;
            }

            // Platform
            var platformProvider = profile.PlatformEnabled
                ? ServiceProviderFactory.CreatePlatform(profile.Platform)
                : new Null.NullPlatformProvider();

            Platform = new PlatformService(platformProvider);
            Platform.Initialize();

            // Ads
            var adsProviders = new List<(IAdsProvider provider, int priority)>();

            if (profile.AdsEnabled && profile.Ads != null)
            {
                foreach (var setting in profile.Ads)
                {
                    if (!setting.Enabled || setting.Provider == ServiceProviderId.None)
                        continue;

                    var provider = ServiceProviderFactory.CreateAds(setting.Provider);
                    if (provider != null && !(provider is Null.NullAdsProvider))
                        adsProviders.Add((provider, setting.Priority));
                }
            }

            Ads = new AdsService(adsProviders);
            Ads.Initialize();

            // Analytics
            var analyticsProviders = new List<IAnalyticsProvider>();

            if (profile.AnalyticsEnabled && profile.Analytics != null)
            {
                foreach (var setting in profile.Analytics)
                {
                    if (!setting.Enabled || setting.Provider == ServiceProviderId.None)
                        continue;

                    var provider = ServiceProviderFactory.CreateAnalytics(setting.Provider);
                    if (provider != null && !(provider is Null.NullAnalyticsProvider))
                        analyticsProviders.Add(provider);
                }
            }

            Analytics = new AnalyticsService(analyticsProviders);
            Analytics.Initialize();
        }
    }
}


