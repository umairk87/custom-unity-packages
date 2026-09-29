#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UK.Framework.SDKIntegrations.Editor
{
    public sealed class SDKIntegrationsBuildProfileWindow : EditorWindow
    {
        private SDKIntegrationsConfig _config;
        private int _profileIndex;
        private string[] _profileIds = new string[0];

        [MenuItem("Tools/UK Framework/SDK Integrations Build Profiles")]
        public static void Open()
        {
            GetWindow<SDKIntegrationsBuildProfileWindow>(
                "SDK Integrations Build Profiles");
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var path = AssetDatabase.FindAssets("t:SDKIntegrationsConfig")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();

            _config = string.IsNullOrEmpty(path)
                ? null
                : AssetDatabase.LoadAssetAtPath<SDKIntegrationsConfig>(path);

            _profileIds = _config?.Profiles
                .Where(x => x != null)
                .Select(x => x.Id)
                .ToArray() ?? new string[0];

            var current = SDKIntegrationsBuildContext.ResolveProfileId();

            _profileIndex = System.Array.IndexOf(_profileIds, current);
            if (_profileIndex < 0)
                _profileIndex = 0;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);

            if (_config == null)
            {
                EditorGUILayout.HelpBox(
                    "SDKIntegrationsConfig asset was not found.",
                    MessageType.Error);

                if (GUILayout.Button("Create Config"))
                    CreateConfig();

                return;
            }

            if (_profileIds.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "No profiles exist in SDKIntegrationsConfig.",
                    MessageType.Warning);
                return;
            }

            _profileIndex = EditorGUILayout.Popup(
                "Build Profile",
                _profileIndex,
                _profileIds);

            var profile = _config.GetProfile(
                _profileIds[_profileIndex]);

            EditorGUILayout.Space(8);
            DrawProfile(profile);

            EditorGUILayout.Space(12);

            if (GUILayout.Button("Use Profile"))
            {
                SDKIntegrationsBuildContext.ProfileId = profile.Id;
                Debug.Log($"[SDKIntegrations] Active profile: {profile.Id}");
            }

            if (GUILayout.Button("Build"))
            {
                SDKIntegrationsBuildContext.ProfileId = profile.Id;
                BuildCurrentTarget(profile);
            }
        }

        private static void DrawProfile(SDKIntegrationsProfile profile)
        {
            EditorGUILayout.LabelField(
                profile.DisplayName,
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                "Platform",
                profile.PlatformEnabled
                    ? profile.Platform.ToString()
                    : "Disabled");

            EditorGUILayout.LabelField(
                "Ads",
                profile.AdsEnabled
                    ? string.Join(", ",
                        profile.Ads
                            .Where(x => x.Enabled)
                            .Select(x => x.Provider.ToString()))
                    : "Disabled");

            EditorGUILayout.LabelField(
                "Analytics",
                profile.AnalyticsEnabled
                    ? string.Join(", ",
                        profile.Analytics
                            .Where(x => x.Enabled)
                            .Select(x => x.Provider.ToString()))
                    : "Disabled");
        }

        private static void BuildCurrentTarget(SDKIntegrationsProfile profile)
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Services Build",
                    "No scenes are configured in Build Settings.",
                    "OK");
                return;
            }

            string path;

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                path = EditorUtility.SaveFolderPanel(
                    "Build " + profile.DisplayName,
                    "Builds",
                    profile.Id);
            }
            else
            {
                path = EditorUtility.SaveFilePanel(
                    "Build " + profile.DisplayName,
                    "Builds",
                    profile.Id,
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android
                        ? "aab"
                        : "");
            }

            if (string.IsNullOrEmpty(path))
                return;

            var scenes = EditorBuildSettings.scenes
                .Where(x => x.enabled)
                .Select(x => x.path)
                .ToArray();

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = path,
                target = EditorUserBuildSettings.activeBuildTarget,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result != BuildResult.Succeeded)
                Debug.LogError("[SDKIntegrations] Build failed.");
        }

        private static void CreateConfig()
        {
            var config = CreateInstance<SDKIntegrationsConfig>();

            config.Profiles.Add(new SDKIntegrationsProfile
            {
                Id = "mobile",
                DisplayName = "Mobile",
                PlatformEnabled = false,
                Platform = ServiceProviderId.None,
                AdsEnabled = true,
                AnalyticsEnabled = false
            });

            config.Profiles.Add(new SDKIntegrationsProfile
            {
                Id = "crazygames",
                DisplayName = "CrazyGames",
                PlatformEnabled = true,
                Platform = ServiceProviderId.CrazyGames,
                AdsEnabled = true,
                AnalyticsEnabled = true,
                Ads =
                {
                    new ProviderSettings
                    {
                        Provider = ServiceProviderId.CrazyGames,
                        Enabled = true,
                        Priority = 0
                    }
                },
                Analytics =
                {
                    new ProviderSettings
                    {
                        Provider = ServiceProviderId.CrazyGames,
                        Enabled = true,
                        Priority = 0
                    }
                }
            });

            config.Profiles.Add(new SDKIntegrationsProfile
            {
                Id = "poki",
                DisplayName = "Poki",
                PlatformEnabled = true,
                Platform = ServiceProviderId.Poki,
                AdsEnabled = true,
                AnalyticsEnabled = true,
                Ads =
                {
                    new ProviderSettings
                    {
                        Provider = ServiceProviderId.Poki,
                        Enabled = true,
                        Priority = 0
                    }
                },
                Analytics =
                {
                    new ProviderSettings
                    {
                        Provider = ServiceProviderId.Poki,
                        Enabled = true,
                        Priority = 0
                    }
                }
            });

            const string folder = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(
                config,
                "Assets/Resources/SDKIntegrationsConfig.asset");

            AssetDatabase.SaveAssets();
        //     LoadConfig();
            Selection.activeObject = config;
        }
    }
}
#endif
