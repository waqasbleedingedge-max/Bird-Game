using UnityEngine;
using System.Collections;

public class ButtonLoopScaleAnim : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 1f;
    public float duration = 0.5f;

    private Coroutine loopRoutine;

    private void OnEnable()
    {
        loopRoutine = StartCoroutine(LoopScaleAnim());
    }

    private void OnDisable()
    {
        if (loopRoutine != null)
            StopCoroutine(loopRoutine);
    }

    private IEnumerator LoopScaleAnim()
    {
        Vector3 small = Vector3.one * minScale;
        Vector3 big = Vector3.one * maxScale;

        while (true)
        {
            // 🔼 Scale up
            yield return StartCoroutine(ScaleTo(small, big));

            // 🔽 Scale down
            yield return StartCoroutine(ScaleTo(big, small));
        }
    }

    private IEnumerator ScaleTo(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // smooth feel
            transform.localScale = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = to;
    }
}
