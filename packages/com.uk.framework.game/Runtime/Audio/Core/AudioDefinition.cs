using System;
using UnityEngine;
using UnityEngine.Audio;

namespace UKFramework.Game.Audio
{
    [Serializable]
    public sealed class AudioDefinition
    {
        public AudioId Id;

        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;

        public Vector2 Pitch =
            new Vector2(0.95f, 1.05f);

        public bool Spatial = true;

        public AudioMixerGroup MixerGroup;
    }
}