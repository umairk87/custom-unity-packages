using System;
using UnityEngine;

namespace UK.Framework.SDKIntegrations
{
    [Serializable]
    public sealed class ProviderSettings
    {
        public ServiceProviderId Provider = ServiceProviderId.None;
        public bool Enabled = true;
        public int Priority = 0;
    }
}
