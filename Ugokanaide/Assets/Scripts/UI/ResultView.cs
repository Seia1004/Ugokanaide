using TMPro;
using UnityEngine;

public class ResultView : MonoBehaviour
{
    [SerializeField] private TMP_Text _survivalTimeText;
    [SerializeField] private TMP_Text _photographedCountText;
    [SerializeField] private TMP_Text _scoreText;

    private void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        _survivalTimeText.text = $"生存時間: {GameManager.Instance.SurvivalTime:F1}秒";
        _photographedCountText.text = $"撮影数: {GameManager.Instance.PhotographedCount}体";
        _scoreText.text = $"スコア: {GameManager.Instance.Score}";
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
