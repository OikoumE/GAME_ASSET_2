using System;
using Controllers;
using Elevator;
using UnityEngine;

namespace StateMachine
{
    public enum GameStateName
    {
        BaseState,
        GameMenuState,
        GameIntroState,
        FuseBoxState,
        WireGameState,
        ExitInShuttleState,
        GameOverState
    }

    public class GameStateMachine : MonoBehaviour
    {
        public PlayerController playerController;
        public ElevatorController elevatorController;
        public LightController lightController;
        public WireGameAudioController wireGameAudioController;
        public bool ENABLECHEAT;


        [SerializeField] public GameStateName currentStateName;
        public readonly GameBaseState exitInShuttleState = new ExitInShuttleState(); //#5
        public readonly GameBaseState fuseBoxState = new FuseBoxState(); //#3
        public readonly GameBaseState gameIntroState = new GameIntroState(); //#2
        public readonly GameBaseState gameMenuState = new GameMenuState(); //#1
        public readonly GameBaseState gameOverState = new GameOverState(); //#6
        public readonly GameBaseState wireGameState = new WireGameState(); //#4
        private GameBaseState currentState;

        public static GameStateMachine Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            if (!playerController) playerController = FindObjectOfType<PlayerController>();
            if (!elevatorController) elevatorController = FindObjectOfType<ElevatorController>();
            if (!lightController) lightController = FindObjectOfType<LightController>();
            if (!lightController) wireGameAudioController = FindObjectOfType<WireGameAudioController>();


            if (!playerController) throw new Exception("MISSING ASSIGNMENT OF: playerController");
            if (!elevatorController) throw new Exception("MISSING ASSIGNMENT OF: elevatorController");
            if (!lightController) throw new Exception("MISSING ASSIGNMENT OF: lightController");
            if (!wireGameAudioController) throw new Exception("MISSING ASSIGNMENT OF: wireGameAudioController");
        }

        private void Start()
        {
            SetState(gameMenuState);
        }

        private void Update()
        {
            currentState.UpdateState(this);
        }

        private void OnValidate()
        {
            if (ENABLECHEAT)
            {
                Debug.Log(currentStateName);

                switch (currentStateName)
                {
                    case GameStateName.BaseState:
                        break;
                    case GameStateName.GameIntroState:
                        SetState(gameIntroState);
                        break;
                    case GameStateName.FuseBoxState:
                        SetState(fuseBoxState);
                        break;
                    case GameStateName.WireGameState:
                        SetState(wireGameState);
                        break;
                    case GameStateName.ExitInShuttleState:
                        SetState(exitInShuttleState);
                        break;
                    case GameStateName.GameOverState:
                        SetState(gameOverState);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsCurrentState(GameStateName checkState)
        {
            return checkState == currentStateName;
        }

        public void SetState(GameBaseState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
        }


        public void SetState(GameStateName newStateName)
        {
            GameBaseState newState;
            switch (newStateName)
            {
                case GameStateName.BaseState:
                case GameStateName.GameIntroState:
                    newState = gameIntroState;
                    break;
                case GameStateName.FuseBoxState:
                    newState = fuseBoxState;
                    break;
                case GameStateName.WireGameState:
                    newState = wireGameState;
                    break;
                case GameStateName.ExitInShuttleState:
                    newState = exitInShuttleState;
                    break;
                case GameStateName.GameOverState:
                    newState = gameOverState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetState(newState);
        }
    }
}