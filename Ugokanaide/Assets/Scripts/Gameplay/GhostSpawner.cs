using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private Transform _target;
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private float _spawnRadius = 8f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnGhost), _spawnInterval, _spawnInterval);
    }

    private void SpawnGhost()
    {
        if (_ghostPrefab == null || _target == null)
        {
            return;
        }

        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = _target.position + (Vector3)(direction * _spawnRadius);
        GameObject ghost = Instantiate(_ghostPrefab, spawnPosition, Quaternion.identity);
        GhostAI ghostAI = ghost.GetComponent<GhostAI>();

        if (ghostAI != null)
        {
            ghostAI.Initialize(_target, _flashlight);
        }
    }
}
