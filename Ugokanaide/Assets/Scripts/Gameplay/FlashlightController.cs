using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light2D _light2D;
    [SerializeField] private float _angleOffsetDegrees = 0f;
    [SerializeField] private float _flashIntensity = 5f;
    [SerializeField] private float _flashFadeSeconds = 0.15f;
    [SerializeField] private float _minAngle = 20f;
    [SerializeField] private float _maxAngle = 90f;
    [SerializeField] private float _minRadius = 3f;
    [SerializeField] private float _maxRadius = 12f;
    [SerializeField] private float _scrollSensitivity = 0.001f;

    private float _baseIntensity;
    private float _zoomLevel = 0.5f;

    private void Awake()
    {
        if (_light2D == null)
        {
            _light2D = GetComponent<Light2D>();
        }

        if (_light2D != null)
        {
            _baseIntensity = _light2D.intensity;
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (_light2D != null)
        {
            float scrollDelta = Mouse.current.scroll.ReadValue().y;
            _zoomLevel = Mathf.Clamp01(
                _zoomLevel + scrollDelta * _scrollSensitivity);
            _light2D.pointLightOuterAngle = Mathf.Lerp(_maxAngle, _minAngle, _zoomLevel);
            _light2D.pointLightOuterRadius = Mathf.Lerp(_minRadius, _maxRadius, _zoomLevel);
            _light2D.pointLightInnerAngle = _light2D.pointLightOuterAngle * 0.4f;
        }

        if (Camera.main == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude == 0f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + _angleOffsetDegrees);
    }

    public bool IsPositionLit(Vector2 worldPosition)
    {
        if (_light2D == null || !_light2D.enabled)
        {
            return false;
        }

        Vector2 lightPosition = transform.position;
        Vector2 direction = worldPosition - lightPosition;
        float distance = Vector2.Distance(lightPosition, worldPosition);
        float angle = Vector2.Angle(transform.up, direction);

        return distance <= _light2D.pointLightOuterRadius
            && angle <= _light2D.pointLightOuterAngle / 2f;
    }

    public void SetLightActive(bool active)
    {
        if (_light2D != null)
        {
            _light2D.enabled = active;

            if (active)
            {
                _light2D.intensity = _baseIntensity;
            }
        }
    }

    public void PlayShutterEffect()
    {
        if (_light2D == null)
        {
            return;
        }

        StartCoroutine(ShutterEffectRoutine());
    }

    private IEnumerator ShutterEffectRoutine()
    {
        _light2D.intensity = _flashIntensity;
        float elapsed = 0f;

        while (elapsed < _flashFadeSeconds)
        {
            elapsed += Time.deltaTime;
            _light2D.intensity = Mathf.Lerp(
                _flashIntensity, 0f, elapsed / _flashFadeSeconds);
            yield return null;
        }

        _light2D.intensity = 0f;
        _light2D.enabled = false;
    }
}
