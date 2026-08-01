using TMPro;
using UnityEngine;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Retry();
        }
    }

    public void OnTitleButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTitle();
        }
    }
}
