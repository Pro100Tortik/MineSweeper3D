using System;
using UnityEngine;

namespace MineSweeper
{
    public class CameraController : MonoBehaviour
    {
        public event Action<string> OnFlagSet;

        [SerializeField] private float speed = 7f;
        [SerializeField] private float sprintSpeed = 15f;
        private AudioManager _audioManager;
        private GameManager _gameManager;
        private int _flagsLeft;

        private void Start()
        {
            _audioManager = AudioManager.Instance;
            _gameManager = GameManager.Instance;
            _gameManager.OnGameStart += SetFlags;
        }

        private void OnDestroy()
        {
            _gameManager.OnGameStart -= SetFlags;
        }

        private void SetFlags()
        {
            _flagsLeft = _gameManager.MinesCurrent;
            OnFlagSet?.Invoke(_flagsLeft.ToString("00"));
        }

        private void Update()
        {
            if (!_gameManager.IsPlaying)
                return;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out ITile tile))
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                        if (tile.Dig())
                            _audioManager.PlayOpenSound();

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (_flagsLeft <= 0)
                            return;

                        if (tile.Flag(out bool placed, out bool removed))
                        {
                            _flagsLeft += placed ? -1 : removed ? 1 : 0;
                            OnFlagSet?.Invoke(_flagsLeft.ToString("00"));
                            _audioManager.PlayFLagSound();
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!_gameManager.IsPlaying)
                return;

            float verticalMovement = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), verticalMovement, Input.GetAxisRaw("Vertical"));
            transform.position += moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed) * Time.deltaTime;
        }
    }
}
