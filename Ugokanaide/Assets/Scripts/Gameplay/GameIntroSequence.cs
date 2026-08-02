using System.Collections;
using UnityEngine;

public class GameIntroSequence : MonoBehaviour
{
    [SerializeField] private Transform _wipeMaskTransform;
    [SerializeField] private GameObject _wipeOverlayRoot;
    [SerializeField] private CanvasGroup[] _fadeCanvasGroups;
    [SerializeField] private AudioClip _startJingle;
    [SerializeField] private float _maxMaskScale = 300f;
    [SerializeField] private float _overrideDurationSeconds = 0f;
    [SerializeField] private float _revealSeconds = 0.35f;
    private bool _hasUnpausedEarly;

    private void Start()
    {
        Time.timeScale = 0f;

        if (_wipeMaskTransform != null)
        {
            _wipeMaskTransform.localScale = Vector3.zero;
        }

        SetCanvasGroupsState(0f, false);

        float duration = _overrideDurationSeconds > 0f
            ? _overrideDurationSeconds
            : (_startJingle != null ? _startJingle.length : 1f);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJingle(_startJingle, () => OnIntroComplete());
        }
        else
        {
            OnIntroComplete();
        }

        StartCoroutine(WipeRoutine(duration));
    }

    private IEnumerator WipeRoutine(float duration)
    {
        float buildupSeconds = Mathf.Max(0f, duration - _revealSeconds);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (elapsed < buildupSeconds)
            {
                yield return null;
                continue;
            }

            if (!_hasUnpausedEarly)
            {
                _hasUnpausedEarly = true;
                Time.timeScale = 1f;
                SetCanvasGroupsState(1f, true);
            }

            float revealElapsed = elapsed - buildupSeconds;
            float p = _revealSeconds > 0f ? Mathf.Clamp01(revealElapsed / _revealSeconds) : 1f;

            if (_wipeMaskTransform != null)
            {
                _wipeMaskTransform.localScale = Vector3.one * Mathf.Lerp(0f, _maxMaskScale, Mathf.Sqrt(p));
            }

            yield return null;
        }
    }

    private void OnIntroComplete()
    {
        Time.timeScale = 1f;

        SetCanvasGroupsState(1f, true);

        if (_wipeOverlayRoot != null)
        {
            _wipeOverlayRoot.SetActive(false);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameBgmChain();
        }
    }

    private void SetCanvasGroupsState(float alpha, bool interactable)
    {
        if (_fadeCanvasGroups == null)
        {
            return;
        }

        foreach (CanvasGroup canvasGroup in _fadeCanvasGroups)
        {
            if (canvasGroup == null)
            {
                continue;
            }

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }
    }
}
