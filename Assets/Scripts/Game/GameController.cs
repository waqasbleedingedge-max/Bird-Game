using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Bird
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Header("Player Data")]
        public int coins;

        [Header("UI")]
        public Text coinText;
        public GameObject pausePanel;

        [Header("Settings")]
        public string mainMenuScene;

        [Header("Sound Button")]
        public Image soundButtonImage;
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;

        [Header("Music Button")]
        public Image musicButtonImage;
        public Sprite musicOnSprite;
        public Sprite musicOffSprite;

        [Header("Loading UI")]
        public GameObject loadingPanel;
        public Image loadingFillImage;



        [SerializeField] private GameObject failedPanel;
        [SerializeField] private GameObject completePanel;



        void Start()
        {
            Time.timeScale = 1f;
            coins = PlayerPrefs.GetInt("coins", 100);
            UpdateCoinText();
        }



        private void OnEnable()
        {
            ParrotController.LevelFailed += LevelFailed;
        }
        private void OnDisable()
        {
            ParrotController.LevelFailed -= LevelFailed;
        }


        void LevelFailed()
        {
            failedPanel.SetActive(true);
        }

        public void OnEnableSettingPanel()
        {
            UpdateSoundIcon();
            UpdateMusicIcon();
        }
        public void AddCoins(int amount)
        {
            coins += amount;
            PlayerPrefs.SetInt("coins", coins);
            UpdateCoinText();
        }

        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                PlayerPrefs.SetInt("coins", coins);
                UpdateCoinText();
                return true;
            }
            return false;
        }

        void UpdateCoinText()
        {
            if (coinText != null)
                coinText.text = coins.ToString();
        }

        // ---------------------------
        // Pause / Resume
        // ---------------------------
        public void PauseGame()
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        // ---------------------------
        // Scene Management
        // ---------------------------
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadSceneAsync(mainMenuScene));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            // ✅ safe checks (NullReference se bachao)
            if (loadingPanel != null) loadingPanel.SetActive(true);
            if (loadingFillImage != null) loadingFillImage.fillAmount = 0f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (loadingFillImage != null)
                    loadingFillImage.fillAmount = progress;

                if (operation.progress >= 0.9f)
                {
                    if (loadingFillImage != null)
                        loadingFillImage.fillAmount = 1f;

                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ---------------------------
        // SOUND & MUSIC
        // ---------------------------
        public void ToggleSound()
        {
            if (SoundController.Instance != null)
                SoundController.Instance.ToggleSound();
            UpdateSoundIcon();
        }

        public void ToggleMusic()
        {
            if (MusicController.Instance != null)
                MusicController.Instance.ToggleMusic();
            UpdateMusicIcon();
        }

        void UpdateSoundIcon()
        {
            if (soundButtonImage == null) return;
            int sound = PlayerPrefs.GetInt("Sound", 1);
            soundButtonImage.sprite = sound == 1 ? soundOnSprite : soundOffSprite;
        }

        void UpdateMusicIcon()
        {
            if (musicButtonImage == null) return;
            int music = PlayerPrefs.GetInt("Music", 1);
            musicButtonImage.sprite = music == 1 ? musicOnSprite : musicOffSprite;
        }



        public void PlayButtonSound()
        {
            SoundController.Instance.PlayButtonSound();
        }
    }
}
