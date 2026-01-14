using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bird
{
    public class MenuController : MonoBehaviour
    {
        [Header("UI")]
        public Text coinText;

        [Header("Scene Names")]
        public string levelSelectionScene;

        [Header("URLs")]
        public string privacyPolicyURL;
        public string moreGamesURL;
        public string rateUsURL;

        [Header("Sound Button")]
        public Image soundButtonImage;
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;

        [Header("Music Button")]
        public Image musicButtonImage;
        public Sprite musicOnSprite;
        public Sprite musicOffSprite;

        void Start()
        {
            if (!PlayerPrefs.HasKey("coins"))
                PlayerPrefs.SetInt("coins", 100);

            UpdateCoinText();
            UpdateSoundIcon();
            UpdateMusicIcon();
        }

        void UpdateCoinText()
        {
            coinText.text = PlayerPrefs.GetInt("coins", 0).ToString();
        }

        // -------------------------
        // BUTTON ACTIONS
        // -------------------------
        public void OnPlayButton()
        {
            SceneManager.LoadScene(levelSelectionScene);
        }

        public void OnRateUsURL()
        {
            Application.OpenURL(rateUsURL);
        }

        public void OnPrivacyURL()
        {
            Application.OpenURL(privacyPolicyURL);
        }

        public void OnMoreGamesURL()
        {
            Application.OpenURL(moreGamesURL);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        // -------------------------
        // SOUND & MUSIC
        // -------------------------
        public void ToggleSound()
        {
            SoundController.Instance.ToggleSound();
            UpdateSoundIcon();
        }

        public void ToggleMusic()
        {
            MusicController.Instance.ToggleMusic();
            UpdateMusicIcon();
        }

        void UpdateSoundIcon()
        {
            int sound = PlayerPrefs.GetInt("Sound", 1);
            soundButtonImage.sprite = sound == 1 ? soundOnSprite : soundOffSprite;
        }

        void UpdateMusicIcon()
        {
            int music = PlayerPrefs.GetInt("Music", 1);
            musicButtonImage.sprite = music == 1 ? musicOnSprite : musicOffSprite;
        }
    }
}
