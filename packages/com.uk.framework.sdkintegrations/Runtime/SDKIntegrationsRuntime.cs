using UnityEngine;

namespace UK.Framework.SDKIntegrations
{
    public static class SDKIntegrationsRuntime
    {
        private static SDKIntegrationsBootstrap _bootstrap;

        public static PlatformService Platform => _bootstrap?.Platform;
        public static AdsService Ads => _bootstrap?.Ads;
        public static AnalyticsService Analytics => _bootstrap?.Analytics;

        public static void Register(SDKIntegrationsBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
        }

        public static void Clear(SDKIntegrationsBootstrap bootstrap)
        {
            if (_bootstrap == bootstrap)
                _bootstrap = null;
        }
    }
}
