using System.Collections.Generic;
using UKFramework.Game.Audio;
using UKFramework.Game.Progress;
using UnityEngine;


namespace UKFramework.Game
{
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private AudioService audioService;

        [SerializeField]
        private GameObject uiService;

        [SerializeField]
        private GameConfig GameConfig;

        private void Awake()
        {
            InitializeApplication();
            InitializeServices();
            InitializeGameInitializers();

        }
        private void InitializeApplication()
        {
            if (GameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned.");
                return;
            }

            Application.targetFrameRate = GameConfig.TargetFrameRate;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void InitializeGameInitializers()
        {


        }

        private void InitializeServices()
        {
            RegisterServices();
            RegisterPoolService();

        }

        private IGameInitializer GetInitializer(GameObject obj)
        {
            var initializer = obj.GetComponent<IGameInitializer>();
            return initializer;
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(GameConfig);

            // uk must do this

            ObjectFactory factory = new ObjectFactory();
            // LevelLoader loader = new LevelLoader(factory);
            LevelService levelService = new LevelService(factory);
            // ServiceLocator.Register(factory);
            // ServiceLocator.Register(loader);
            ServiceLocator.Register(levelService);

            ServiceLocator.Register<IProgressService>(new ProgressService());
            audioService.Initialize(GameConfig.AudioDatabase);

            InitializeGameplayState();
            InitializeGameState();
            GetInitializer(uiService)?.Initialize();
            //  uiService.Initialize();

        }

        private void RegisterPoolService()
        {
            PoolService poolService = new PoolService();
            ServiceLocator.Register(poolService);
        }

        private void InitializeGameplayState()
        {
            IGameplayState gameplayState = new GameplayState(18, 3);
            ServiceLocator.Register<IGameplayState>(gameplayState);
        }
        private void InitializeGameState()
        {
            IGameStateService gameState = new GameStateService();
            ServiceLocator.Register<IGameStateService>(gameState);
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();

        }




    }

}