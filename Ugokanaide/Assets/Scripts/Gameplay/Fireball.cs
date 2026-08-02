using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    [SerializeField] private float _maxLifetimeSeconds = 6f;
    [SerializeField] private RuntimeAnimatorController _extinguishController;
    [SerializeField] private float _extinguishDestroyDelaySeconds = 0.4f;
    [SerializeField] private AudioSource _extinguishAudioSource;
    [SerializeField] private AudioClip _seExtinguish;
    [SerializeField] private float _minExtinguishPitch = 0.9f;
    [SerializeField] private float _maxExtinguishPitch = 1.1f;
    [SerializeField] private float _rotationOffsetDegrees = 0f;
    [SerializeField] private Light2D _light;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Collider2D _collider;
    private FlashlightController _flashlight;
    private Vector2 _direction;
    private float _speed;
    private bool _isExtinguished;
    private float _elapsedSeconds;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 direction, FlashlightController flashlight, float speed)
    {
        _direction = direction.normalized;
        _flashlight = flashlight;
        _speed = speed;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + _rotationOffsetDegrees);
    }

    private void Update()
    {
        if (_isExtinguished)
        {
            return;
        }

        _elapsedSeconds += Time.deltaTime;
        _rigidbody.position += _direction * _speed * Time.deltaTime;

        if (_flashlight != null && _flashlight.IsPositionLit(_rigidbody.position))
        {
            Extinguish();
            return;
        }

        if (_elapsedSeconds >= _maxLifetimeSeconds)
        {
            Destroy(gameObject);
        }
    }

    private void Extinguish()
    {
        if (_isExtinguished)
        {
            return;
        }

        _isExtinguished = true;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        if (_animator != null && _extinguishController != null)
        {
            _animator.runtimeAnimatorController = _extinguishController;
        }

        if (_extinguishAudioSource != null && _seExtinguish != null)
        {
            _extinguishAudioSource.pitch = Random.Range(_minExtinguishPitch, _maxExtinguishPitch);
            _extinguishAudioSource.PlayOneShot(_seExtinguish);
        }

        if (_light != null)
        {
            StartCoroutine(FadeLightRoutine());
        }

        Destroy(gameObject, _extinguishDestroyDelaySeconds);
    }

    private IEnumerator FadeLightRoutine()
    {
        float startIntensity = _light.intensity;
        float elapsed = 0f;

        while (elapsed < _extinguishDestroyDelaySeconds)
        {
            elapsed += Time.deltaTime;
            _light.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / _extinguishDestroyDelaySeconds);
            yield return null;
        }
    }
}
