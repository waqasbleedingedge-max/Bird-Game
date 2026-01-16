using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bird
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Header("Player Data")]
        [SerializeField] private int coins = 100;

        [Header("UI")]
        [SerializeField] private Text coinText;
        [SerializeField] private GameObject pausePanel;

        [Header("Settings")]
        [SerializeField] private string mainMenuScene = "MainMenu";

        [Header("Sound Button")]
        [SerializeField] private Image soundButtonImage;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;

        [Header("Music Button")]
        [SerializeField] private Image musicButtonImage;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("Loading UI")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Image loadingFillImage;

        [Header("Panels")]
        [SerializeField] private GameObject failedPanel;
        [SerializeField] private GameObject completePanel;

        [Header("LEVEL PREFABS (10)")]
        [SerializeField] private GameObject[] levelPrefabs;
        private GameObject currentLevel;

        [Header("LEVEL INDEX SAVE KEY")]
        [SerializeField] private string levelKey = "levelToPlay";
        private int levelIndex; // 0-based

        [Header("⏳ TIMER (10 values in seconds)")]
        [SerializeField] private float[] levelTimers;
        [SerializeField] private Text timerText;
        private float currentTimer;
        private bool timerRunning;
        private int lastShownSecond = -1;

        [Header("🐦 BIRD SPAWN POINTS (10)")]
        [SerializeField] private Transform[] birdSpawnPoints;
        [SerializeField] private Transform bird;
        private Rigidbody birdRb;

        // [Header("🎬 CUTSCENE TIMELINES (Optional 10)")]
        // [SerializeField] private UnityEngine.Playables.PlayableDirector[] levelTimelines = new UnityEngine.Playables.PlayableDirector[10];

        private bool levelEnded;



        [Header("LEVEL UI")]
        [SerializeField] private Text levelText;
        [SerializeField] private string levelTextFormat = "Level {0}/{1}";
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        private void Start()
        {
            Time.timeScale = 1f;

            coins = PlayerPrefs.GetInt("coins", coins);
            UpdateCoinText();

            if (bird != null) bird.TryGetComponent(out birdRb);

            int saved = PlayerPrefs.GetInt(levelKey, 1);
            levelIndex = Mathf.Clamp(saved - 1, 0, levelPrefabs.Length - 1);

            StartLevel(levelIndex);
        }

        private void Update()
        {
            if (levelEnded) return;

            TickTimer();
            CheckFallFail();
        }

        // =========================
        // TIMER (optimized)
        // =========================
        private void TickTimer()
        {
            if (!timerRunning) return;

            currentTimer -= Time.deltaTime;
            if (currentTimer < 0f) currentTimer = 0f;

            // ✅ Update UI only when second changes
            int secNow = Mathf.CeilToInt(currentTimer);
            if (secNow != lastShownSecond)
            {
                lastShownSecond = secNow;
                UpdateTimerUI();
            }

            if (currentTimer <= 0f)
                LevelFailed();
        }

        private void UpdateTimerUI()
        {
            if (timerText == null) return;

            int min = Mathf.FloorToInt(currentTimer / 60f);
            int sec = Mathf.FloorToInt(currentTimer % 60f);
            timerText.text = $"{min:00}:{sec:00}";
        }
        private void UpdateLevelUI()
        {
            if (levelText == null) return;

            int levelNumber = levelIndex + 1;
            int totalLevels = (levelPrefabs != null) ? levelPrefabs.Length : 0;

            levelText.text = string.Format(levelTextFormat, levelNumber, totalLevels);
        }

        private void CheckFallFail()
        {
            if (bird != null && bird.position.y < -0.5f)
                LevelFailed();
        }

        // =========================
        // LEVEL SYSTEM
        // =========================
        private void StartLevel(int index)
        {
            levelEnded = false;
            Time.timeScale = 1f;

            HidePanels();
            UpdateLevelUI();   // ✅ ADD THIS

            SpawnLevelPrefab(index);
            SpawnBirdAtPoint(index);
            StartLevelTimer(index);

            // ---------------- CUTSCENE (commented for now) ----------------
            // PlayTimeline(index);
        }

        private void SpawnLevelPrefab(int index)
        {
            if (currentLevel != null)
                Destroy(currentLevel);

            if (levelPrefabs == null || index < 0 || index >= levelPrefabs.Length) return;
            if (levelPrefabs[index] == null) return;

            currentLevel = Instantiate(levelPrefabs[index]);
        }

        private void SpawnBirdAtPoint(int index)
        {
            if (bird == null) return;
            if (birdSpawnPoints == null || index < 0 || index >= birdSpawnPoints.Length) return;

            Transform sp = birdSpawnPoints[index];
            if (sp == null) return;

            bird.SetPositionAndRotation(sp.position, sp.rotation);

            if (birdRb != null)
            {
                birdRb.linearVelocity = Vector3.zero;
                birdRb.angularVelocity = Vector3.zero;
            }
        }

        private void StartLevelTimer(int index)
        {
            timerRunning = true;

            float duration = 60f;
            if (levelTimers != null && index >= 0 && index < levelTimers.Length && levelTimers[index] > 0f)
                duration = levelTimers[index];

            currentTimer = duration;
            lastShownSecond = -1; // force refresh
            UpdateTimerUI();
        }

        private void HidePanels()
        {
            if (failedPanel) failedPanel.SetActive(false);
            if (completePanel) completePanel.SetActive(false);
        }

        // ---------------- CUTSCENE (commented for now) ----------------
        /*
        private void PlayTimeline(int index)
        {
            if (levelTimelines == null || index < 0 || index >= levelTimelines.Length) return;
            if (levelTimelines[index] == null) return;

            levelTimelines[index].Stop();
            levelTimelines[index].Play();
        }
        */

        // =========================
        // COMPLETE / FAIL
        // =========================
        public void LevelComplete()
        {
            if (levelEnded) return;
            levelEnded = true;

            timerRunning = false;
            if (completePanel) completePanel.SetActive(true);

            Time.timeScale = 0f;
        }

        public void LevelFailed()
        {
            if (levelEnded) return;
            levelEnded = true;

            timerRunning = false;
            if (failedPanel) failedPanel.SetActive(true);

            Time.timeScale = 0f;
        }

        public void NextLevel()
        {
            Time.timeScale = 1f;

            levelIndex++;
            if (levelIndex >= levelPrefabs.Length)
                levelIndex = 0;

            PlayerPrefs.SetInt(levelKey, levelIndex + 1);
            StartLevel(levelIndex);
        }

        public void RestartLevel()
        {
            StartLevel(levelIndex);
        }

        // =========================
        // COINS
        // =========================
        public void AddCoins(int amount)
        {
            coins += amount;
            PlayerPrefs.SetInt("coins", coins);
            UpdateCoinText();
        }

        public bool SpendCoins(int amount)
        {
            if (coins < amount) return false;

            coins -= amount;
            PlayerPrefs.SetInt("coins", coins);
            UpdateCoinText();
            return true;
        }

        private void UpdateCoinText()
        {
            if (coinText != null)
                coinText.text = coins.ToString();
        }

        // =========================
        // PAUSE
        // =========================
        public void PauseGame()
        {
            Time.timeScale = 0f;
            if (pausePanel) pausePanel.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            if (pausePanel) pausePanel.SetActive(false);
        }

        // =========================
        // MENU / LOADING
        // =========================
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(mainMenuScene))
                StartCoroutine(LoadSceneAsync(mainMenuScene));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingPanel) loadingPanel.SetActive(true);
            if (loadingFillImage) loadingFillImage.fillAmount = 0f;

            var op = SceneManager.LoadSceneAsync(sceneName);
            if (op == null) yield break;

            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                if (loadingFillImage) loadingFillImage.fillAmount = progress;

                if (op.progress >= 0.9f)
                {
                    if (loadingFillImage) loadingFillImage.fillAmount = 1f;
                    op.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        // =========================
        // SOUND / MUSIC
        // =========================
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

        private void UpdateSoundIcon()
        {
            if (soundButtonImage == null) return;
            int sound = PlayerPrefs.GetInt("Sound", 1);
            soundButtonImage.sprite = sound == 1 ? soundOnSprite : soundOffSprite;
        }

        private void UpdateMusicIcon()
        {
            if (musicButtonImage == null) return;
            int music = PlayerPrefs.GetInt("Music", 1);
            musicButtonImage.sprite = music == 1 ? musicOnSprite : musicOffSprite;
        }

        public void PlayButtonSound()
        {
            if (SoundController.Instance != null)
                SoundController.Instance.PlayButtonSound();
        }
    }
}
