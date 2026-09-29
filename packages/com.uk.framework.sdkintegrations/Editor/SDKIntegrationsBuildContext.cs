#if UNITY_EDITOR
using UnityEditor;

namespace UK.Framework.SDKIntegrations.Editor
{
    public static class SDKIntegrationsBuildContext
    {
        private const string SessionKey = "UK.Framework.SDKIntegrations.Profile";

        public static string ProfileId
        {
            get => SessionState.GetString(SessionKey, string.Empty);
            set => SessionState.SetString(SessionKey, value ?? string.Empty);
        }

        public static void Clear()
        {
            SessionState.EraseString(SessionKey);
        }

        public static string GetCommandLineProfile()
        {
            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--profile")
                    return args[i + 1];
            }

            return string.Empty;
        }

        public static string ResolveProfileId(string defaultProfile = "mobile")
        {
            var cli = GetCommandLineProfile();
            if (!string.IsNullOrWhiteSpace(cli))
                return cli;

            if (!string.IsNullOrWhiteSpace(ProfileId))
                return ProfileId;

            return defaultProfile;
        }
    }
}
#endif
