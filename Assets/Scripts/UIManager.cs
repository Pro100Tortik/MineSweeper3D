using TMPro;
using UnityEngine;

namespace MineSweeper
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text flagsText;
        [SerializeField] private CameraController cameraController;
        private GameManager _gameManager;
        private float _minutes;
        private float _seconds;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameManager.OnGameStart += ResetTimer;
        }

        private void OnEnable() => cameraController.OnFlagSet += SetFlagsText;

        private void OnDestroy()
        {
            _gameManager.OnGameStart -= ResetTimer;
            cameraController.OnFlagSet -= SetFlagsText;
        }

        private void Update()
        {
            UpdateTimer();
            if (_minutes < 100)
                timerText.text = $"{_minutes:00}:{_seconds:00}";
            else
                timerText.text = $"SUCKS";
        }

        private void SetFlagsText(string flagsLeft) => flagsText.text = flagsLeft;

        private void ResetTimer()
        {
            _minutes = 0;
            _seconds = 0;
            timerText.text = "00:00";
        }

        private void UpdateTimer()
        {
            if (!_gameManager.IsPlaying)
                return;

            if (_gameManager.IsPaused)
                return;

            _seconds += Time.deltaTime;
            if (_seconds >= 60)
            {
                _seconds = 0;
                _minutes++;
            }
        }
    }
}
