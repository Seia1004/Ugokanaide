using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private Transform _target;
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private float _spawnMargin = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnGhost), _spawnInterval, _spawnInterval);
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
        GameObject ghost = Instantiate(_ghostPrefab, spawnPosition, Quaternion.identity);
        GhostAI ghostAI = ghost.GetComponent<GhostAI>();

        if (ghostAI != null)
        {
            ghostAI.Initialize(_target, _flashlight);
        }
    }
}
