using System;
using Controllers;
using Elevator;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public class AudioSourceSettings
    {
        [SerializeField] public AudioSource Source;
        [SerializeField] public AudioClip audioClip;
        [SerializeField] [Range(0, 1)] public float volume = 1f;
        [SerializeField] [Range(-3, 3)] public float pitch = 1f;
    }

    public enum GameStateName
    {
        BaseState,
        GameMenuState,
        GameIntroState,
        FuseBoxState,
        WireGameState,
        ExitInShuttleState,
        GameOverState,
        GamePauseState
    }

    public class GameStateMachine : MonoBehaviour
    {
        public PlayerController playerController;
        public ElevatorController elevatorController;
        public LightController lightController;
        public WireGameAudioController wireGameAudioController;
        public MainMenuHandler mainMenuHandler;

        public bool ENABLECHEAT;
        [SerializeField] public GameStateName currentStateName;

        [SerializeField] private GameObject start, resume, menu;

        public readonly GameBaseState
            gameMenuState = new GameMenuState(),
            gameIntroState = new GameIntroState(),
            gamePauseState = new GamePauseState(),
            fuseBoxState = new FuseBoxState(),
            wireGameState = new WireGameState(),
            exitInShuttleState = new ExitInShuttleState(),
            gameOverState = new GameOverState();

        private GameBaseState currentState;
        private bool isPaused;

        public GameStateName PrevState { get; private set; }

        public static GameStateMachine Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            InitOnAwake();
        }

        private void Start()
        {
            SetState(gameMenuState);
        }

        private void Update()
        {
            CursorLockHandler();
            currentState?.UpdateState(this);
        }

        private void OnValidate()
        {
            if (ENABLECHEAT)
            {
                Debug.Log(currentStateName);
                SetState(currentStateName);
            }
        }


        private void CursorLockHandler()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            OpenPauseMenu();
        }

        public void OpenPauseMenu()
        {
            isPaused = !isPaused;
            var cursorLock = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            SetCursorLockMode(cursorLock);

            start.SetActive(false);
            resume.SetActive(isPaused);
            menu.SetActive(isPaused);
            playerController.playerHasControl = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }

        public void ToggleCursorLockMode()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        private void InitOnAwake()
        {
            if (!playerController) playerController = FindObjectOfType<PlayerController>();
            if (!elevatorController) elevatorController = FindObjectOfType<ElevatorController>();
            if (!lightController) lightController = FindObjectOfType<LightController>();
            if (!lightController) wireGameAudioController = FindObjectOfType<WireGameAudioController>();
            if (!mainMenuHandler) mainMenuHandler = FindObjectOfType<MainMenuHandler>();


            if (!playerController) throw new Exception("MISSING ASSIGNMENT OF: playerController");
            if (!elevatorController) throw new Exception("MISSING ASSIGNMENT OF: elevatorController");
            if (!lightController) throw new Exception("MISSING ASSIGNMENT OF: lightController");
            if (!wireGameAudioController) throw new Exception("MISSING ASSIGNMENT OF: wireGameAudioController");
            if (!mainMenuHandler) throw new Exception("MISSING ASSIGNMENT OF: mainMenuHandler");


            PrevState = gameIntroState.gameStateName;
        }

        /// <summary>
        ///     if setState is passed, defaults to CursorLockMode.Locked;
        /// </summary>
        /// <param name="state">what state to set if setState = true</param>
        public void SetCursorLockMode(CursorLockMode state)
        {
            Cursor.lockState = state;
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.visible = false;
        }

        public void ExitGame()
        {
            Debug.Log("EXIT GAME");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public bool IsCurrentState(GameStateName checkState)
        {
            return checkState == currentStateName;
        }

        public void SetState(GameBaseState newState)
        {
            if (currentState != null)
            {
                PrevState = currentState.gameStateName;
                currentState.ExitState(this);
            }

            currentState = newState;
            currentState.EnterState(this);
        }


        public void SetState(GameStateName newStateName)
        {
            var newState = newStateName switch
            {
                GameStateName.BaseState => gameMenuState,
                GameStateName.GameMenuState => gameMenuState,
                GameStateName.GameIntroState => gameIntroState,
                GameStateName.FuseBoxState => fuseBoxState,
                GameStateName.WireGameState => wireGameState,
                GameStateName.ExitInShuttleState => exitInShuttleState,
                GameStateName.GameOverState => gameOverState,
                _ => throw new ArgumentOutOfRangeException(nameof(newStateName), newStateName, "NO CASE FOUND!")
            };

            SetState(newState);
        }
    }
}