using System;
using System.Collections.Generic;
using System.Linq;

namespace UK.Framework.SDKIntegrations
{
    public sealed class AdsService
    {
        private sealed class Entry
        {
            public IAdsProvider Provider;
            public int Priority;
        }

        private readonly List<Entry> _providers;

        public AdsService(IEnumerable<(IAdsProvider provider, int priority)> providers)
        {
            _providers = providers
                .Where(x => x.provider != null)
                .Select(x => new Entry
                {
                    Provider = x.provider,
                    Priority = x.priority
                })
                .OrderBy(x => x.Priority)
                .ToList();
        }

        public void Initialize()
        {
            foreach (var entry in _providers)
                entry.Provider.Initialize(null);
        }

        public bool ShowInterstitial(Action<AdsShowResult> callback = null)
        {
            foreach (var entry in _providers)
            {
                if (!entry.Provider.IsInitialized)
                    continue;

                if (entry.Provider.ShowInterstitial(callback))
                    return true;
            }

            callback?.Invoke(AdsShowResult.NotAvailable);
            return false;
        }

        public bool ShowRewarded(Action<AdsShowResult> callback = null)
        {
            foreach (var entry in _providers)
            {
                if (!entry.Provider.IsInitialized)
                    continue;

                if (entry.Provider.ShowRewarded(callback))
                    return true;
            }

            callback?.Invoke(AdsShowResult.NotAvailable);
            return false;
        }

        public bool ShowBanner()
        {
            foreach (var entry in _providers)
            {
                if (!entry.Provider.IsInitialized)
                    continue;

                if (entry.Provider.ShowBanner())
                    return true;
            }

            return false;
        }

        public void HideBanner()
        {
            foreach (var entry in _providers)
                entry.Provider.HideBanner();
        }
    }
}
