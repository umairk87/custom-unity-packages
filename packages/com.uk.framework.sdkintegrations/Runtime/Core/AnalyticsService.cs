using System.Collections.Generic;
using System.Linq;

namespace UK.Framework.SDKIntegrations
{
    public sealed class AnalyticsService
    {
        private readonly List<IAnalyticsProvider> _providers;

        public AnalyticsService(IEnumerable<IAnalyticsProvider> providers)
        {
            _providers = providers?.Where(x => x != null).ToList()
                         ?? new List<IAnalyticsProvider>();
        }

        public void Initialize()
        {
            foreach (var provider in _providers)
            {
                if (provider.IsAvailable)
                    provider.Initialize();
            }
        }

        public void TrackEvent(
            string eventName,
            string parameter = null,
            string value = null)
        {
            foreach (var provider in _providers)
            {
                if (provider.IsAvailable)
                    provider.TrackEvent(eventName, parameter, value);
            }
        }
    }
}
