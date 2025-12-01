using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class HiddenWall : MonoBehaviour
{
    [SerializeField] private float transparentAlpha = 0.3f;
    [SerializeField] private float fadeDuration = 0.3f;
    private Tilemap tilemap;
    private Color originalColor;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        tilemap = GetComponentInParent<Tilemap>();
        originalColor = tilemap.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(transparentAlpha);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(originalColor.a);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToAlpha(targetAlpha));
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        float startAlpha = tilemap.color.a;
        float time = 0f;
        Color c = tilemap.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            tilemap.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        tilemap.color = c;
        fadeCoroutine = null;
    }
}