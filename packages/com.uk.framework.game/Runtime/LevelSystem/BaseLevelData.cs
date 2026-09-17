using UnityEngine;
using System.Collections.Generic;


namespace UKFramework.Game
{

    [CreateAssetMenu(
        fileName = "Level",
        menuName = "Game/BaseLevel")]
    public abstract class BaseLevelData : ScriptableObject
    {
        [SerializeField]
        private int levelNumber;
        public int LevelNumber => levelNumber;
        public abstract void Load(LevelService levelService, ObjectFactory factory, Transform parentRoot);


    }

}