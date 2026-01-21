using UnityEngine;

namespace Bird
{
    public class MusicController : MonoBehaviour
    {
        public static MusicController Instance;

        [Header("Audio Source")]
        public AudioSource musicSource;

        [Header("Music Clip")]
        public AudioClip backgroundMusic;

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
            if (!PlayerPrefs.HasKey("Music"))
                PlayerPrefs.SetInt("Music", 1);

            PlayMusic();
        }

        // -------------------------
        // PLAY MUSIC (DEBUG ONLY)
        // -------------------------
        void PlayMusic()
        {
            if (PlayerPrefs.GetInt("Music") == 0)
            {
                return;
            }


             musicSource.clip = backgroundMusic;
             musicSource.loop = true;
             musicSource.Play();
        }

        // -------------------------
        // MUSIC ON / OFF
        // -------------------------
        public void ToggleMusic()
        {
            int music = PlayerPrefs.GetInt("Music");

            if (music == 1)
            {
                PlayerPrefs.SetInt("Music", 0);
                Debug.Log("🔇 Music OFF");

                 musicSource.Stop();
            }
            else
            {
                PlayerPrefs.SetInt("Music", 1);

                PlayMusic();
            }
        }
    }
}
