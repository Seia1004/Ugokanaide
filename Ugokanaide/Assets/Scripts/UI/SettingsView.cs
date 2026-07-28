using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _seVolumeSlider;
    [SerializeField] private GameObject _panel;

    private float _previousTimeScale = 1f;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    public void OnBgmVolumeChanged(float value)
    {
        _audioMixer.SetFloat("BGMVolume", ToDecibel(value));
    }

    public void OnSeVolumeChanged(float value)
    {
        _audioMixer.SetFloat("SEVolume", ToDecibel(value));
    }

    public void Open()
    {
        SyncSlidersFromMixer();
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _panel.SetActive(true);
    }

    public void Close()
    {
        _panel.SetActive(false);
        Time.timeScale = _previousTimeScale;
    }

    public void Toggle()
    {
        if (_panel.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void SyncSlidersFromMixer()
    {
        if (_audioMixer.GetFloat("BGMVolume", out float bgmDb))
        {
            _bgmVolumeSlider.SetValueWithoutNotify(ToLinear(bgmDb));
        }

        if (_audioMixer.GetFloat("SEVolume", out float seDb))
        {
            _seVolumeSlider.SetValueWithoutNotify(ToLinear(seDb));
        }
    }

    private static float ToDecibel(float linear)
    {
        return Mathf.Log10(Mathf.Max(linear, 0.0001f)) * 20f;
    }

    private static float ToLinear(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
}
