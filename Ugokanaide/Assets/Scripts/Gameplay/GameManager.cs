using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _resultView;
    [SerializeField] private AudioClip _seGhostCaught;
    [SerializeField] private AudioClip _seFireballCaught;

    public static GameManager Instance { get; private set; }
    public bool IsGameOver { get; private set; }
    public float SurvivalTime { get; private set; }
    public int PhotographedCount { get; private set; }
    public int Score => Mathf.FloorToInt(SurvivalTime) * PhotographedCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(LogStatus), 1f, 1f);
    }

    private void Update()
    {
        if (!IsGameOver)
        {
            SurvivalTime += Time.deltaTime;
        }
    }

    private void LogStatus()
    {
        if (IsGameOver)
        {
            return;
        }

        Debug.Log($"生存時間: {SurvivalTime:F1}秒 / 撮影数: {PhotographedCount}体 / スコア: {Score}");
    }

    public void EndGame(bool killedByFireball = false)
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(killedByFireball ? _seFireballCaught : _seGhostCaught);
            AudioManager.Instance.PlayResultJingle();
        }

        if (_resultView != null)
        {
            _resultView.SetActive(true);
        }
    }

    public void RegisterPhotographed(int count)
    {
        PhotographedCount += count;
    }

    public void Retry()
    {
#if UNITY_EDITOR
        System.Type logEntriesType = System.Type.GetType("UnityEditor.LogEntries, UnityEditor");
        System.Reflection.MethodInfo clearMethod = logEntriesType?.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod?.Invoke(null, null);
#endif

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
