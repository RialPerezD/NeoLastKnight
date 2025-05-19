using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class FadeInFadeOut : MonoBehaviour
{
    public RawImage rawImage;
    public TextMeshProUGUI text;
    public float fadeDuration = 1f;
    public float timetilfade = 1f;
    private GameManager instance;
    private bool faded = false;

    void Start()
    {
        // Set to transparent

        // Start fading in
        //StartCoroutine(FadeOut());
        instance = FindFirstObjectByType<GameManager>();
        if (instance)
        {
            if (instance.tutorial) SetAlpha(0f);
            else
            {
                SetAlpha(1f);
                string levelname = (instance.siguienteNivel - 1).ToString();

                text.text = "Level: " + levelname;

            }
        }

    }

    void SetAlpha(float alpha)
    {
        if (rawImage)
        {
            Color c = rawImage.color;
            c.a = alpha;
            rawImage.color = c;
        }

        if (text)
        {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        faded = true;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsed / fadeDuration)); // goes from 1 to 0
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f); // Ensure fully transparent at the end
    }

    void Update()
    {
        timetilfade -= Time.deltaTime;
        if(timetilfade < 0 && !faded && !instance.tutorial) StartCoroutine(FadeOut());
    }
}
