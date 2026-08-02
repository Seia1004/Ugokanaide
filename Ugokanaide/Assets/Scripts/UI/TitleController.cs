using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [SerializeField] private GameObject _rulesPanel;
    [SerializeField] private AudioClip _seStart;
    [SerializeField] private AudioClip _seRulesOpen;
    [SerializeField] private AudioClip _seRulesClose;
    [SerializeField] private RulesPageNavigator _rulesPageNavigator;

    public void OnStartButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySEThen(_seStart, () => SceneManager.LoadScene("Game"));
        }
        else
        {
            SceneManager.LoadScene("Game");
        }

        if (ScreenFader.Instance != null && _seStart != null)
        {
            ScreenFader.Instance.FadeToBlack(_seStart.length);
        }
    }

    public void OnRulesButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(_seRulesOpen);
        }

        if (_rulesPageNavigator != null)
        {
            _rulesPageNavigator.ResetToFirstPage();
        }

        if (_rulesPanel != null)
        {
            _rulesPanel.SetActive(true);
        }
    }

    public void OnCloseRulesButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(_seRulesClose);
        }

        if (_rulesPanel != null)
        {
            _rulesPanel.SetActive(false);
        }
    }
}
