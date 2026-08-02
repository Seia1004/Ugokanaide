using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _seSource;
    [SerializeField] private AudioClip _sceneBgm;
    [SerializeField] private AudioClip _bgmCursedArmour;
    [SerializeField] private AudioClip _bgmCursedArmourV6;
    [SerializeField] private AudioClip _jingleItsCursed;
    [SerializeField] private AudioClip _jingleLoss;

    private Coroutine _gameBgmChainCoroutine;

    public static AudioManager Instance { get; private set; }
    public bool IsLoopTrackActive { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayBGM(_sceneBgm);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || _bgmSource == null)
        {
            return;
        }

        _bgmSource.Stop();
        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.Play();
    }

    public void StopBgm()
    {
        if (_gameBgmChainCoroutine != null)
        {
            StopCoroutine(_gameBgmChainCoroutine);
            _gameBgmChainCoroutine = null;
        }

        if (_bgmSource != null)
        {
            _bgmSource.Stop();
        }

        IsLoopTrackActive = false;
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null || _seSource == null)
        {
            return;
        }

        _seSource.PlayOneShot(clip);
    }

    public void PlaySEThen(AudioClip clip, Action onComplete)
    {
        PlaySE(clip);
        StartCoroutine(PlaySEThenRoutine(clip, onComplete));
    }

    public void PlayJingle(AudioClip clip, Action onComplete = null)
    {
        if (clip == null)
        {
            onComplete?.Invoke();
            return;
        }

        PlayBGM(clip, false);
        StartCoroutine(JingleRoutine(clip.length, onComplete));
    }

    public void PlayGameBgmChain()
    {
        if (_gameBgmChainCoroutine != null)
        {
            StopCoroutine(_gameBgmChainCoroutine);
        }

        _gameBgmChainCoroutine = StartCoroutine(GameBgmChainRoutine());
    }

    public void PlayResultJingle()
    {
        bool wasLoopActive = IsLoopTrackActive;
        StopBgm();

        if (wasLoopActive)
        {
            PlayJingle(_jingleLoss);
        }
        else
        {
            PlayJingle(_jingleItsCursed);
        }
    }

    private IEnumerator JingleRoutine(float duration, Action onComplete)
    {
        yield return new WaitForSecondsRealtime(duration);
        onComplete?.Invoke();
    }

    private IEnumerator PlaySEThenRoutine(AudioClip clip, Action onComplete)
    {
        if (clip != null)
        {
            yield return new WaitForSecondsRealtime(clip.length);
        }

        onComplete?.Invoke();
    }

    private IEnumerator GameBgmChainRoutine()
    {
        IsLoopTrackActive = false;
        PlayBGM(_bgmCursedArmour, false);

        while (_bgmSource != null && _bgmSource.isPlaying)
        {
            yield return null;
        }

        PlayBGM(_bgmCursedArmourV6);
        IsLoopTrackActive = true;
        _gameBgmChainCoroutine = null;
    }
}
