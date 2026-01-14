using UnityEngine;

namespace Bird
{
    public class SoundController : MonoBehaviour
    {
        public static SoundController Instance;

        [Header("Audio Source")]
        public AudioSource sfxSource;

        [Header("Sound Clips")]
        public AudioClip buttonClick;
        public AudioClip winSound;
        public AudioClip loseSound;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            if (!PlayerPrefs.HasKey("Sound"))
                PlayerPrefs.SetInt("Sound", 1);
        }

        // -------------------------
        // SFX PLAY (DEBUG ONLY)
        // -------------------------
        public void PlayButtonSound()
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                Debug.Log("🔇 Sound OFF → Button sound not played");
                return;
            }

            Debug.Log("🔊 Button click sound triggered");

            // sfxSource.PlayOneShot(buttonClick);
        }

        public void PlayWinSound()
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                Debug.Log("🔇 Sound OFF → Win sound not played");
                return;
            }

            Debug.Log("🏆 Win sound triggered");

            // sfxSource.PlayOneShot(winSound);
        }

        public void PlayLoseSound()
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                Debug.Log("🔇 Sound OFF → Lose sound not played");
                return;
            }

            Debug.Log("💀 Lose sound triggered");

            // sfxSource.PlayOneShot(loseSound);
        }

        // -------------------------
        // SOUND ON / OFF
        // -------------------------
        public void ToggleSound()
        {
            int sound = PlayerPrefs.GetInt("Sound");
            sound = sound == 1 ? 0 : 1;
            PlayerPrefs.SetInt("Sound", sound);

            Debug.Log(sound == 1 ? "🔊 Sound ON" : "🔇 Sound OFF");
        }
    }
}
