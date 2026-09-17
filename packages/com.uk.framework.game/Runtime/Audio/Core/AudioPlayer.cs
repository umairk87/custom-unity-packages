using UnityEngine;

namespace UKFramework.Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;

        public bool IsPlaying => audioSource.isPlaying;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioDefinition definition, Vector3 position)
        {
            transform.position = position;
            audioSource.clip = definition.Clip;
            audioSource.volume = definition.Volume;

            audioSource.pitch = Random.Range(
                definition.Pitch.x,
                definition.Pitch.y
            );

            audioSource.spatialBlend =
            definition.Spatial ? 1f : 0f;

            audioSource.outputAudioMixerGroup =
            definition.MixerGroup;

            audioSource.loop = false;

            audioSource.Play();
        }

        public void Stop()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}