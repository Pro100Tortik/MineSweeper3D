using System;
using UnityEngine;

namespace MineSweeper
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;
        private static GameManager _instance;

        public event Action OnGameStart;
        public event Action OnGameEnd;

        public bool IsPlaying => gameState == GameState.Playing;
        public bool IsPaused => gameState == GameState.Waiting;
        public int MinesCurrent => mines;

        public GameState CurrentGameState => gameState;
        [SerializeField] private int fieldX = 30;
        [SerializeField] private int fieldY = 20;
        [SerializeField] private int mines = 40;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameState gameState;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            gridManager.OnWin += Win;
            gridManager.OnLose += Lose;
        }

        private void OnDisable()
        {
            gridManager.OnWin -= Win;
            gridManager.OnLose -= Lose;
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
            gridManager.StartGame(fieldX, fieldY, mines);
            OnGameStart?.Invoke();
        }

        private void Win()
        {
            gameState = GameState.Win;
            OnGameEnd?.Invoke();
        }

        private void Lose()
        {
            gameState = GameState.Lose;
            OnGameEnd?.Invoke();
        }
    }
}
