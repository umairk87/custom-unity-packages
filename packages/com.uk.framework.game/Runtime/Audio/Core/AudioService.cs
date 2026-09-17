using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Data.Common;
using UKFramework.Game.Progress;
using UKFramework.Game.Events;

namespace UKFramework.Game.Audio
{
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Configuration")]
        private AudioDatabase database;

        [SerializeField]
        private int initialPoolSize = 16;

        private AudioPlayer audioPlayerPrefab;
        private AudioSource musicSource;
        private AudioMixer audioMixer;

        private readonly List<AudioPlayer> players = new List<AudioPlayer>();

        private bool initialized;
        private IProgressService progressService;

        public void InitializeDB(AudioDatabase db)
        {
            database = db;
            audioMixer = db.AudioMixer;
            audioPlayerPrefab = db.AudioPlayerPrefab;

        }
        public void Initialize(AudioDatabase db)
        {
            if (initialized)
                return;

            if (db == null)
            {
                Debug.LogError(
                    "AudioService: AudioDatabase is missing.");

                return;
            }

            // if (audioPlayerPrefab == null)
            // {
            //     Debug.LogError(
            //         "AudioService: AudioPlayer prefab is missing.");

            //     return;
            // }
            InitializeDB(db);
            database.Initialize();
            CreatePool();
            ServiceLocator.Register<IAudioService>(this);
            progressService = ServiceLocator.Get<IProgressService>();
            RegisterEvents();
            initialized = true;

        }


        void Start()
        {
            ApplySavedSettings();
        }

        private void RegisterEvents()
        {
            EventBus.Subscribe<MusicSettingChangedData>(
             OnMusicSettingChanged);

            EventBus.Subscribe<SoundSettingChangedData>(
                OnSoundSettingChanged);

        }

        private void ApplySavedSettings()
        {
            Debug.Log("progressService.MusicEnabled " + progressService.MusicEnabled);
            SetMusicVolume(progressService.MusicEnabled);
            SetSoundVolume(progressService.SoundEnabled);
            // PlayMusic(AudioId.BackgroundMusic);

        }

        private void OnMusicSettingChanged(MusicSettingChangedData data)
        {
            SetMusicVolume(data.Enabled);
        }

        private void OnSoundSettingChanged(
            SoundSettingChangedData data)
        {
            SetSoundVolume(data.Enabled);
        }

        private void SetMusicVolume(bool enabled)
        {
            Debug.Log("SetMusicVolume " + enabled);
            audioMixer.SetFloat(
                database.MusicVolume,
                enabled ? 0f : -80f);
        }

        private void SetSoundVolume(bool enabled)
        {
            float volume = enabled ? 0f : -80f;

            audioMixer.SetFloat(database.SFXVolume, volume);
            audioMixer.SetFloat(database.UIVolume, volume);
        }

        private void CreatePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreatePlayer();
            }
        }


        private AudioPlayer CreatePlayer()
        {
            AudioPlayer player =
                Instantiate(
                    audioPlayerPrefab,
                    transform);

            player.gameObject.SetActive(false);

            players.Add(player);

            return player;
        }
        private AudioSource CreateMusicSource()
        {
            AudioPlayer player =
               Instantiate(
                   audioPlayerPrefab,
                   transform);

            player.name = "MusicAudioSource";
            var source = player.GetComponent<AudioSource>();
            return source;
        }


        public void Play(AudioId id)
        {
            Play(id, Vector3.zero);
        }


        public void Play(
            AudioId id,
            Vector3 position)
        {
            if (!initialized)
            {
                Debug.LogWarning(
                    "AudioService is not initialized.");

                return;
            }

            if (!database.TryGet(
                id,
                out AudioDefinition definition))
            {
                return;
            }

            AudioPlayer player =
                GetAvailablePlayer();

            player.gameObject.SetActive(true);

            player.Play(
                definition,
                position);
        }


        private AudioPlayer GetAvailablePlayer()
        {
            foreach (AudioPlayer player in players)
            {
                if (!player.IsPlaying)
                    return player;
            }

            return CreatePlayer();
        }


        public void PlayMusic(AudioId id)
        {
            if (!database.TryGet(
                id,
                out AudioDefinition definition))
            {
                return;
            }
            if (musicSource == null)
            {
                musicSource = CreateMusicSource();
            }

            musicSource.clip =
                definition.Clip;

            musicSource.volume =
                definition.Volume;

            musicSource.loop = true;

            musicSource.outputAudioMixerGroup =
                definition.MixerGroup;

            musicSource.Play();
        }


        public void StopMusic()
        {
            musicSource.Stop();
        }


        public void StopAllSFX()
        {
            foreach (AudioPlayer player in players)
            {
                player.Stop();

                player.gameObject.SetActive(false);
            }
        }
    }
}