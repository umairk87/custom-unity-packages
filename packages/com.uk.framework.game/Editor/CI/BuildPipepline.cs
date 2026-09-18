#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UK.Framework.Game.Editor.CI
{
    public static class BuildPipepline
    {
        public static void Build()
        {
            string targetName = GetArgument(
                "-buildTarget",
                "StandaloneWindows64");

            string environment = GetArgument(
                "-environment",
                "Development");

            Debug.Log($"[CI] Build Target: {targetName}");
            Debug.Log($"[CI] Environment: {environment}");

            BuildTarget target = ParseBuildTarget(targetName);

            ConfigureEnvironment(environment);

            string[] scenes = GetScenes();

            if (scenes.Length == 0)
            {
                Debug.LogError("[CI] No enabled scenes found in Build Settings.");
                EditorApplication.Exit(1);
                return;
            }

            string outputPath = GetOutputPath(target);

            PrepareOutputDirectory(outputPath);

            Debug.Log($"[CI] Output: {outputPath}");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            BuildReport report =
                UnityEditor.BuildPipeline.BuildPlayer(buildOptions);

            BuildSummary summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                Debug.LogError(
                    $"[CI] Build FAILED\n" +
                    $"Result: {summary.result}\n" +
                    $"Errors: {summary.totalErrors}\n" +
                    $"Warnings: {summary.totalWarnings}"
                );

                EditorApplication.Exit(1);
                return;
            }

            Debug.Log(
                $"[CI] Build SUCCEEDED\n" +
                $"Target: {target}\n" +
                $"Size: {summary.totalSize / (1024f * 1024f):F2} MB\n" +
                $"Time: {summary.totalTime}"
            );

            EditorApplication.Exit(0);
        }

        // ---------------------------------------------------------
        // Scenes
        // ---------------------------------------------------------

        private static string[] GetScenes()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .Where(path => !string.IsNullOrEmpty(path))
                .ToArray();
        }

        // ---------------------------------------------------------
        // Environment
        // ---------------------------------------------------------

        private static void ConfigureEnvironment(string environment)
        {
            switch (environment.ToLowerInvariant())
            {
                case "development":

                    Debug.Log("[CI] Using Development environment");

                    break;

                case "staging":

                    Debug.Log("[CI] Using Staging environment");

                    break;

                case "production":

                    Debug.Log("[CI] Using Production environment");

                    break;

                default:

                    throw new Exception(
                        $"Unknown environment: {environment}");
            }
        }

        // ---------------------------------------------------------
        // Build Target
        // ---------------------------------------------------------

        private static BuildTarget ParseBuildTarget(string value)
        {
            if (Enum.TryParse(
                    value,
                    true,
                    out BuildTarget target))
            {
                return target;
            }

            throw new Exception(
                $"Unsupported Unity BuildTarget: {value}");
        }

        // ---------------------------------------------------------
        // Output
        // ---------------------------------------------------------

        private static string GetOutputPath(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:

                    return "Builds/Android/Game.apk";

                case BuildTarget.WebGL:

                    return "Builds/WebGL";

                case BuildTarget.StandaloneWindows64:

                    return "Builds/Windows/Game.exe";

                case BuildTarget.StandaloneOSX:

                    return "Builds/macOS/Game.app";

                case BuildTarget.StandaloneLinux64:

                    return "Builds/Linux/Game.x86_64";

                default:

                    return $"Builds/{target}/Game";
            }
        }

        // ---------------------------------------------------------
        // Directory
        // ---------------------------------------------------------

        private static void PrepareOutputDirectory(string outputPath)
        {
            string directory =
                Path.GetDirectoryName(outputPath);

            if (string.IsNullOrEmpty(directory))
                return;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        // ---------------------------------------------------------
        // Command Line
        // ---------------------------------------------------------

        private static string GetArgument(
            string argument,
            string defaultValue)
        {
            string[] args =
                Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(
                        argument,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return defaultValue;
        }
    }
}

#endif