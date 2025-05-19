using UnityEngine;
using System.Collections;

public class FadeInFadeOut : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvas(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvas(1f, 0f));
    }

    private IEnumerator FadeCanvas(float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = end;

        // Optionally disable interaction when faded out
        canvasGroup.interactable = end > 0.9f;
        canvasGroup.blocksRaycasts = end > 0.9f;
    }
}
