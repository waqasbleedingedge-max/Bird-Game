using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Bird
{
    public class LoadingScreen : MonoBehaviour
    {
        public Image fillBar;
        public Text progressText;
        public string SceneName;
        public float delayBeforeLoad = 1.2f;  // Delay after full load
        public float smoothSpeed = 2f;        // Smoothness speed

        private float smoothProgress = 0f;    // Smooth fill amount

        void Start()
        {
            StartCoroutine(LoadSceneAsync());
        }

        IEnumerator LoadSceneAsync()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneName);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                // Unity reports 0 → 0.9 while loading
                float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

                // Smoothly move our bar towards real progress
                smoothProgress = Mathf.Lerp(smoothProgress, targetProgress, Time.deltaTime * smoothSpeed);

                // Apply to UI
                fillBar.fillAmount = smoothProgress;
                progressText.text ="LOADING: "+ Mathf.RoundToInt(smoothProgress * 100f) + " %";

                // When both real and smooth reach 100%
                if (smoothProgress >= 0.99f && targetProgress >= 1f)
                {
                    yield return new WaitForSeconds(delayBeforeLoad);
                    op.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
