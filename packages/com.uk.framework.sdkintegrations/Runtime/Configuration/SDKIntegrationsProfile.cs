using System;
using System.Collections.Generic;

namespace UK.Framework.SDKIntegrations
{
    [Serializable]
    public sealed class SDKIntegrationsProfile
    {
        public string Id;
        public string DisplayName;

        public bool PlatformEnabled = true;
        public ServiceProviderId Platform = ServiceProviderId.None;

        public bool AdsEnabled = true;
        public List<ProviderSettings> Ads = new();

        public bool AnalyticsEnabled = true;
        public List<ProviderSettings> Analytics = new();

        public ProviderSettings GetAdsProvider(ServiceProviderId id)
        {
            return Ads?.Find(x => x.Provider == id);
        }

        public ProviderSettings GetAnalyticsProvider(ServiceProviderId id)
        {
            return Analytics?.Find(x => x.Provider == id);
        }
    }
}
