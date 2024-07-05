using UnityEngine;

namespace MineSweeper
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance => _instance;
        private static AudioManager _instance;

        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip flagSound;
        [SerializeField] private AudioClip boomSound;
        [SerializeField] private AudioClip winSound;

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
        }

        public void PlayOpenSound() => soundSource.PlayOneShot(openSound);
        public void PlayFLagSound() => soundSource.PlayOneShot(flagSound);
        public void PlayBoomSound() => soundSource.PlayOneShot(boomSound);
        public void PlayWinSound() => soundSource.PlayOneShot(winSound);
    }
}
