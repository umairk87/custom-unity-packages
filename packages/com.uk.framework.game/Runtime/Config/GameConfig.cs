using UnityEngine;
using UKFramework.Game.Audio;

namespace UKFramework.Game
{
    [CreateAssetMenu(
        fileName = "GameConfig",
        menuName = "Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField] private AudioDatabase audioDatabase;

        [Header("Application")]
        [SerializeField] private int targetFrameRate = 60;

        public AudioDatabase AudioDatabase => audioDatabase;
        public int TargetFrameRate => targetFrameRate;

    }
}