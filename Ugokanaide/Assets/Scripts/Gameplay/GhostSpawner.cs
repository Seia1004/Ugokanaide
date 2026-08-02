using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private GameObject _bigGhostPrefab;
    [SerializeField] private float _bigGhostChance = 0.15f;
    [SerializeField] private Transform _target;
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _initialSpawnRatePer10Seconds = 2f;
    [SerializeField] private float _maxSpawnRatePer10Seconds = 6f;
    [SerializeField] private float _rampUpDurationSeconds = 120f;
    [SerializeField] private bool _capSpawnRate = true;
    [SerializeField] private float _spawnMargin = 2f;

    private float _elapsedSinceStart;
    private float _spawnTimer;

    private void Update()
    {
        _elapsedSinceStart += Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        float rampT = _rampUpDurationSeconds > 0f ? _elapsedSinceStart / _rampUpDurationSeconds : 1f;

        if (_capSpawnRate)
        {
            rampT = Mathf.Clamp01(rampT);
        }

        float currentRatePer10Seconds = Mathf.LerpUnclamped(
            _initialSpawnRatePer10Seconds,
            _maxSpawnRatePer10Seconds,
            rampT);
        currentRatePer10Seconds = Mathf.Max(currentRatePer10Seconds, 0.01f);
        float currentInterval = 10f / currentRatePer10Seconds;

        if (_spawnTimer >= currentInterval)
        {
            _spawnTimer -= currentInterval;
            SpawnGhost();
        }
    }

    private void SpawnGhost()
    {
        Camera cam = Camera.main;

        if (_ghostPrefab == null || _target == null || cam == null)
        {
            return;
        }

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 cameraPosition = cam.transform.position;
        float outerHalfWidth = halfWidth + _spawnMargin;
        float outerHalfHeight = halfHeight + _spawnMargin;
        int side = Random.Range(0, 4);
        float offsetX;
        float offsetY;

        switch (side)
        {
            case 0:
                offsetX = Random.Range(-outerHalfWidth, outerHalfWidth);
                offsetY = outerHalfHeight;
                break;
            case 1:
                offsetX = Random.Range(-outerHalfWidth, outerHalfWidth);
                offsetY = -outerHalfHeight;
                break;
            case 2:
                offsetX = -outerHalfWidth;
                offsetY = Random.Range(-outerHalfHeight, outerHalfHeight);
                break;
            default:
                offsetX = outerHalfWidth;
                offsetY = Random.Range(-outerHalfHeight, outerHalfHeight);
                break;
        }

        Vector3 spawnPosition = new Vector3(
            cameraPosition.x + offsetX,
            cameraPosition.y + offsetY,
            0f);
        bool spawnBig = _bigGhostPrefab != null && Random.value < _bigGhostChance;
        GameObject prefabToSpawn = spawnBig ? _bigGhostPrefab : _ghostPrefab;
        GameObject ghost = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        GhostAI ghostAI = ghost.GetComponent<GhostAI>();

        if (ghostAI != null)
        {
            ghostAI.Initialize(_target, _flashlight);
        }

        GhostFireballLauncher fireballLauncher = ghost.GetComponent<GhostFireballLauncher>();

        if (fireballLauncher != null)
        {
            fireballLauncher.Initialize(_target, _flashlight);
        }
    }
}
