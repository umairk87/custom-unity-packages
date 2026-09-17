using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UKFramework.Game.Audio
{
    [CreateAssetMenu(
        menuName = "Game/Audio/Audio Database")]
    public sealed class AudioDatabase : ScriptableObject
    {
        [SerializeField]
        private List<AudioDefinition> sounds = new List<AudioDefinition>();

        [Header("Mixer & Player")]
        [SerializeField]
        private AudioPlayer audioPlayerPrefab;
        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private string musicVolume = "MusicVolume";
        [SerializeField] private string sfxVolume = "SFXVolume";
        [SerializeField] private string uiVolume = "UIVolume";

        public AudioPlayer AudioPlayerPrefab => audioPlayerPrefab;
        public AudioMixer AudioMixer => audioMixer;
        public string MusicVolume => musicVolume;
        public string SFXVolume => sfxVolume;
        public string UIVolume => uiVolume;
        private Dictionary<AudioId, AudioDefinition> lookup;


        public void Initialize()
        {
            lookup = new Dictionary<AudioId, AudioDefinition>();

            foreach (AudioDefinition sound in sounds)
            {
                if (sound == null)
                    continue;

                if (sound.Id == AudioId.None)
                    continue;

                if (sound.Clip == null)
                    continue;

                lookup[sound.Id] = sound;
            }
        }

        public bool TryGet(AudioId id, out AudioDefinition definition)
        {
            definition = null;

            if (lookup == null)
            {
                Initialize();
            }
            return lookup.TryGetValue(
                id,
                out definition);
        }
    }
}