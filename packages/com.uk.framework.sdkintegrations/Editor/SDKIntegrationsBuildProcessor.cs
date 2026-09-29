#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UK.Framework.SDKIntegrations.Editor
{
    public sealed class SDKIntegrationsBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
            var config = LoadConfig();

            if (config == null)
            {
                Debug.LogWarning(
                    "[SDKIntegrations] SDKIntegrationsConfig not found. " +
                    "Build will use no providers.");
                return;
            }

            var profileId = SDKIntegrationsBuildContext.ResolveProfileId();
            var profile = config.GetProfile(profileId);

            if (profile == null)
            {
                throw new BuildFailedException(
                    $"[SDKIntegrations] Profile '{profileId}' was not found.");
            }

            Validate(report.summary.platform, profile);
            ConfigureDefines(profile);

            Debug.Log($"[SDKIntegrations] Build profile: {profile.Id}");
        }

        private static SDKIntegrationsConfig LoadConfig()
        {
            return AssetDatabase.FindAssets("t:SDKIntegrationsConfig")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SDKIntegrationsConfig>)
                .FirstOrDefault(x => x != null);
        }

        private static void Validate(
            BuildTarget target,
            SDKIntegrationsProfile profile)
        {
            if (target == BuildTarget.WebGL)
            {
                if (profile.Platform == ServiceProviderId.CrazyGames &&
                    !profile.PlatformEnabled)
                {
                    throw new BuildFailedException(
                        "[SDKIntegrations] CrazyGames profile has platform disabled.");
                }

                if (profile.Platform == ServiceProviderId.Poki &&
                    !profile.PlatformEnabled)
                {
                    throw new BuildFailedException(
                        "[SDKIntegrations] Poki profile has platform disabled.");
                }
            }
        }

        private static void ConfigureDefines(SDKIntegrationsProfile profile)
        {
            var group = BuildPipeline.GetBuildTargetGroup(
                EditorUserBuildSettings.activeBuildTarget);

            if (group == BuildTargetGroup.Unknown)
                return;

            // We deliberately only configure SERVICE defines here.
            // SDK defines (UK_SDK_*) are owned by your SDK detection/AutoDefine system.
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                .Split(';')
                .Where(x => !x.StartsWith("UK_SERVICE_"))
                .ToList();

            void Add(string symbol)
            {
                if (!symbols.Contains(symbol))
                    symbols.Add(symbol);
            }

            if (profile.PlatformEnabled)
                AddServiceDefine(profile.Platform);

            if (profile.AdsEnabled)
                foreach (var x in profile.Ads.Where(x => x.Enabled))
                    AddServiceDefine(x.Provider);

            if (profile.AnalyticsEnabled)
                foreach (var x in profile.Analytics.Where(x => x.Enabled))
                    AddServiceDefine(x.Provider);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                group,
                string.Join(";", symbols));
        }

        private static void AddServiceDefine(ServiceProviderId id)
        {
            if (id == ServiceProviderId.None)
                return;

            AddServiceDefineRaw(id);
        }

        private static void AddServiceDefineRaw(ServiceProviderId id)
        {
            var group = BuildPipeline.GetBuildTargetGroup(
                EditorUserBuildSettings.activeBuildTarget);

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                .Split(';')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var symbol = $"UK_SERVICE_{id.ToString().ToUpperInvariant()}";

            if (!symbols.Contains(symbol))
                symbols.Add(symbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                group,
                string.Join(";", symbols));
        }
    }
}
#endif
