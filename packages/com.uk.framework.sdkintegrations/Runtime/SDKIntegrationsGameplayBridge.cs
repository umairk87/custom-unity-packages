using UnityEngine;

namespace UK.Framework.SDKIntegrations
{
    /// <summary>
    /// Optional helper for wiring your existing GameState system.
    /// Call these methods from your existing state transitions rather
    /// than creating a second game-state system.
    /// </summary>
    public sealed class SDKIntegrationsGameplayBridge : MonoBehaviour
    {
        public void LoadingFinished()
        {
            SDKIntegrationsRuntime.Platform?.GameLoadingFinished();
        }

        public void GameplayStarted()
        {
            SDKIntegrationsRuntime.Platform?.GameplayStart();
        }

        public void GameplayStopped()
        {
            SDKIntegrationsRuntime.Platform?.GameplayStop();
        }

        public void LevelStarted(int level)
        {
            SDKIntegrationsRuntime.Analytics?.TrackEvent(
                "level",
                level.ToString(),
                "start");
        }

        public void LevelCompleted(int level)
        {
            SDKIntegrationsRuntime.Analytics?.TrackEvent(
                "level",
                level.ToString(),
                "complete");
        }

        public void LevelFailed(int level)
        {
            SDKIntegrationsRuntime.Analytics?.TrackEvent(
                "level",
                level.ToString(),
                "fail");
        }
    }
}
