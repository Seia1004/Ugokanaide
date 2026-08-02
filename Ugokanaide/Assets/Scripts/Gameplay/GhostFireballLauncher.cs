using System.Collections;
using UnityEngine;

public class GhostFireballLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private float _checkIntervalSeconds = 1f;
    [SerializeField] private float _fireProbability = 0.08f;
    [SerializeField] private float _aimErrorDegrees = 3f;
    [SerializeField] private float _baseSpeed = 14f;
    [SerializeField] private float _speedVariance = 3f;
    [SerializeField] private float _minimumFireDistance = 5f;
    [SerializeField] private float _windUpSeconds = 0.4f;
    [SerializeField] private float _postFireStopSeconds = 0.5f;
    [SerializeField] private AudioSource _fireLaunchAudioSource;
    [SerializeField] private AudioClip _seFireLaunch;
    [SerializeField] private float _minLaunchPitch = 0.85f;
    [SerializeField] private float _maxLaunchPitch = 1.25f;

    private Transform _target;
    private FlashlightController _flashlight;
    private GhostAI _ghostAI;

    public void Initialize(Transform target, FlashlightController flashlight)
    {
        _target = target;
        _flashlight = flashlight;
        _ghostAI = GetComponent<GhostAI>();
        StartCoroutine(FireLoop());
    }

    public void StopFiring()
    {
        StopAllCoroutines();
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_checkIntervalSeconds);

            if (IsEligibleToFire() && Random.value < _fireProbability)
            {
                StartCoroutine(FireRoutine());
            }
        }
    }

    private IEnumerator FireRoutine()
    {
        if (_ghostAI != null)
        {
            _ghostAI.SetMovementPaused(true);
            _ghostAI.PlayThrowTrigger();
        }

        float elapsed = 0f;

        while (elapsed < _windUpSeconds)
        {
            if (_ghostAI != null && _ghostAI.IsLit)
            {
                _ghostAI.SetMovementPaused(false);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Fire();

        yield return new WaitForSeconds(_postFireStopSeconds);

        if (_ghostAI != null)
        {
            _ghostAI.SetMovementPaused(false);
        }
    }

    private bool IsEligibleToFire()
    {
        if (_ghostAI != null && _ghostAI.IsLit)
        {
            return false;
        }

        Camera cam = Camera.main;

        if (cam == null || _target == null)
        {
            return false;
        }

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 cameraPosition = cam.transform.position;
        Vector3 offset = transform.position - cameraPosition;
        bool insideView = Mathf.Abs(offset.x) <= halfWidth && Mathf.Abs(offset.y) <= halfHeight;

        if (!insideView)
        {
            return false;
        }

        float distanceToTarget = Vector2.Distance(transform.position, _target.position);

        return distanceToTarget >= _minimumFireDistance;
    }

    private void Fire()
    {
        if (_fireballPrefab == null || _target == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        float randomAngle = Random.Range(-_aimErrorDegrees, _aimErrorDegrees);
        direction = Quaternion.Euler(0f, 0f, randomAngle) * direction;

        float speed = _baseSpeed + Random.Range(-_speedVariance, _speedVariance);

        GameObject fireballObject = Instantiate(_fireballPrefab, transform.position, Quaternion.identity);
        Fireball fireball = fireballObject.GetComponent<Fireball>();

        if (fireball != null)
        {
            fireball.Initialize(direction, _flashlight, speed);
        }

        if (_fireLaunchAudioSource != null && _seFireLaunch != null)
        {
            float speedT = _speedVariance > 0f
                ? Mathf.InverseLerp(_baseSpeed - _speedVariance, _baseSpeed + _speedVariance, speed)
                : 0.5f;
            _fireLaunchAudioSource.pitch = Mathf.Lerp(_minLaunchPitch, _maxLaunchPitch, speedT);
            _fireLaunchAudioSource.PlayOneShot(_seFireLaunch);
        }
    }
}
