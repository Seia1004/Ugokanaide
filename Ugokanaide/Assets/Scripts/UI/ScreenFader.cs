using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image _overlayImage;
    [SerializeField] private float _fadeInOnStartSeconds = 0f;

    public static ScreenFader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_overlayImage == null)
        {
            return;
        }

        if (_fadeInOnStartSeconds > 0f)
        {
            SetAlpha(1f);
            StartCoroutine(FadeRoutine(1f, 0f, _fadeInOnStartSeconds, null));
        }
        else
        {
            SetAlpha(0f);
        }
    }

    public void FadeToBlack(float duration, Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration, onComplete));
    }

    private void SetAlpha(float alpha)
    {
        if (_overlayImage == null)
        {
            return;
        }

        Color color = _overlayImage.color;
        color.a = alpha;
        _overlayImage.color = color;
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration)));
            yield return null;
        }

        SetAlpha(to);
        onComplete?.Invoke();
    }
}
