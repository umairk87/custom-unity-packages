using UnityEngine;

namespace UKFramework.Game.Audio
{
    public interface IAudioService
    {
        void Play(AudioId id);

        void Play(AudioId id, Vector3 position);

        void PlayMusic(AudioId id);

        void StopMusic();

        void StopAllSFX();
    }
}