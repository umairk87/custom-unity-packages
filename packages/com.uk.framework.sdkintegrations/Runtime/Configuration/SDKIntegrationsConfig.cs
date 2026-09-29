using System.Collections.Generic;
using UnityEngine;

namespace UK.Framework.SDKIntegrations
{
    [CreateAssetMenu(
        fileName = "SDKIntegrationsConfig",
        menuName = "UK Framework/SDK Integrations Config")]
    public sealed class SDKIntegrationsConfig : ScriptableObject
    {
        public List<SDKIntegrationsProfile> Profiles = new();

        public SDKIntegrationsProfile GetProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return Profiles?.Find(x =>
                x != null &&
                string.Equals(x.Id, id, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
