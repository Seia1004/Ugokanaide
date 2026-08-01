using TMPro;
using UnityEngine;

public class ScoreHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    private void Update()
    {
        if (_scoreText == null || GameManager.Instance == null)
        {
            return;
        }

        _scoreText.text = $"Score: {GameManager.Instance.Score}";
    }
}
