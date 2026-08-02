using TMPro;
using UnityEngine;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private AudioClip _seRetry;
    [SerializeField] private AudioClip _seTitle;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        _scoreText.text = GameManager.Instance.Score.ToString();
    }

    public void OnRetryButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySEThen(_seRetry, () =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Retry();
                }
            });
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.Retry();
        }

        if (ScreenFader.Instance != null && _seRetry != null)
        {
            ScreenFader.Instance.FadeToBlack(_seRetry.length);
        }
    }

    public void OnTitleButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySEThen(_seTitle, () =>
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ReturnToTitle();
                }
            });
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTitle();
        }
    }
}
