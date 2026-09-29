#if UNITY_EDITOR
using UnityEditor;

namespace UK.Framework.SDKIntegrations.Editor
{
    public static class SDKIntegrationsCLI
    {
        // Call this from your existing UnityCLI before BuildPipeline.BuildPlayer:
        //
        // SDKIntegrationsCLI.ApplyCommandLineProfile();
        //
        // Example:
        // ./unity build webgl --profile crazygames

        public static string ApplyCommandLineProfile()
        {
            var profile = SDKIntegrationsBuildContext.GetCommandLineProfile();

            if (!string.IsNullOrWhiteSpace(profile))
            {
                SDKIntegrationsBuildContext.ProfileId = profile;
            }

            return SDKIntegrationsBuildContext.ResolveProfileId();
        }

        public static void SetProfile(string profileId)
        {
            SDKIntegrationsBuildContext.ProfileId = profileId;
        }

        public static string GetProfile()
        {
            return SDKIntegrationsBuildContext.ResolveProfileId();
        }
    }
}
#endif
