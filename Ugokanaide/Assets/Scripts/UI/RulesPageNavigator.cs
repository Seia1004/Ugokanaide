using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RulesPageNavigator : MonoBehaviour
{
    [SerializeField] private GameObject[] _pages;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioClip _sePrev;
    [SerializeField] private AudioClip _seNext;

    private int _currentPageIndex;
    private AudioSource _fallbackSeSource;

    private void Awake()
    {
        _fallbackSeSource = GetComponent<AudioSource>();
        if (_fallbackSeSource == null)
        {
            _fallbackSeSource = gameObject.AddComponent<AudioSource>();
        }
        _fallbackSeSource.playOnAwake = false;
        _fallbackSeSource.loop = false;
        _fallbackSeSource.spatialBlend = 0f;

        if (_audioMixer == null)
        {
            return;
        }

        AudioMixerGroup[] seGroups = _audioMixer.FindMatchingGroups("SE");
        if (seGroups.Length > 0)
        {
            _fallbackSeSource.outputAudioMixerGroup = seGroups[0];
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            Previous();
        }
        else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            Next();
        }
    }

    public void ResetToFirstPage()
    {
        _currentPageIndex = 0;
        UpdatePageVisibility();
    }

    public void Next()
    {
        if (_pages == null || _currentPageIndex >= _pages.Length - 1)
        {
            return;
        }

        _currentPageIndex++;

        PlayPageSe(_seNext);

        UpdatePageVisibility();
    }

    public void Previous()
    {
        if (_pages == null || _currentPageIndex <= 0)
        {
            return;
        }

        _currentPageIndex--;

        PlayPageSe(_sePrev);

        UpdatePageVisibility();
    }

    private void PlayPageSe(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(clip);
            return;
        }

        if (_fallbackSeSource != null)
        {
            _fallbackSeSource.PlayOneShot(clip);
        }
    }

    private void UpdatePageVisibility()
    {
        int pageCount = _pages == null ? 0 : _pages.Length;

        for (int i = 0; i < pageCount; i++)
        {
            if (_pages[i] != null)
            {
                _pages[i].SetActive(i == _currentPageIndex);
            }
        }

        if (_prevButton != null)
        {
            _prevButton.interactable = pageCount > 0 && _currentPageIndex > 0;
        }

        if (_nextButton != null)
        {
            _nextButton.interactable = pageCount > 0 && _currentPageIndex < pageCount - 1;
        }
    }
}
