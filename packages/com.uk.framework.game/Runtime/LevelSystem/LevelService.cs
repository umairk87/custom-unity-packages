using UnityEngine;

namespace UKFramework.Game
{
    public class LevelService
    {
        // private LevelLoader loader;
        private ObjectFactory factory;
        public LevelService(ObjectFactory fact)
        {
            //  this.loader = loader;
            this.factory = fact;
        }
        // public LevelService(LevelLoader loader, ObjectFactory fact)
        // {
        //     //  this.loader = loader;
        //     this.factory = fact;
        // }

        public void Build(BaseLevelData level, Transform parentRoot)
        {
            level.Load(this, factory, parentRoot);
            //   loader.Load(level, parentRoot);

        }


    }

}