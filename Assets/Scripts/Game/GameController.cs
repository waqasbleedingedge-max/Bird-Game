using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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





        private void OnEnable()
        {

        }
        private void OnDisable()
        {

        }


        void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Load coins from PlayerPrefs
            coins = PlayerPrefs.GetInt("coins", 100);
            UpdateCoinUI();
        }






        // ---------------------------
        // Coin Management
        // ---------------------------
        public void AddCoins(int amount)
        {
            coins += amount;
            PlayerPrefs.SetInt("coins", coins);
            UpdateCoinUI();
        }

        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                PlayerPrefs.SetInt("coins", coins);
                UpdateCoinUI();
                return true;
            }
            return false; // Not enough coins
        }

        void UpdateCoinUI()
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
            Time.timeScale = 1f; // Make sure time is normal
            SceneManager.LoadScene(mainMenuScene);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
