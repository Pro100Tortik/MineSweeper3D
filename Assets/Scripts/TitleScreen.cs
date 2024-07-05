using UnityEngine;
using UnityEngine.UI;

namespace MineSweeper
{
    public class TitleScreen : MonoBehaviour
    {
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private GameObject settingsScreen;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button settingsGameButton;
        [SerializeField] private Button quitGameButton;
        private GameManager _gameManager;

        private void Awake()
        {
            titleScreen.SetActive(true);
            settingsScreen.SetActive(false);
            gameScreen.SetActive(false);

            startGameButton.onClick.AddListener(StartGame);
            settingsGameButton.onClick.AddListener(OpenSettings);
            quitGameButton.onClick.AddListener(Quit);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameManager.OnGameEnd += GoBack;
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveAllListeners();
            settingsGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.RemoveAllListeners();
            _gameManager.OnGameEnd -= GoBack;
        }

        private void StartGame()
        {
            _gameManager.StartGame();
            gameScreen.SetActive(true);
            titleScreen.SetActive(false);
        }

        public void GoBack()
        {
            settingsScreen.SetActive(false);
            titleScreen.SetActive(true);
            gameScreen.SetActive(false);
        }

        private void OpenSettings()
        {
            titleScreen.SetActive(false);
            settingsScreen.SetActive(true);
        }

        private void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}
