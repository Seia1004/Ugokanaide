using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ShutterController : MonoBehaviour
{
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _chargeSeconds = 3f;

    private bool _isCharging;
    private bool _isPointerOverUI;
    private float _chargeElapsed;

    public float ChargeProgress => _isCharging ? Mathf.Clamp01(_chargeElapsed / _chargeSeconds) : 1f;

    private void Update()
    {
        _isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public void OnAttack(InputValue value)
    {
        if (_isCharging)
        {
            return;
        }

        if (!value.isPressed)
        {
            return;
        }

        if (Time.timeScale == 0f)
        {
            return;
        }

        if (_isPointerOverUI)
        {
            return;
        }

        GhostAI[] ghosts = FindObjectsByType<GhostAI>(FindObjectsSortMode.None);
        int destroyedCount = 0;

        foreach (GhostAI ghost in ghosts)
        {
            if (ghost.IsLit)
            {
                ghost.Die();
                destroyedCount += ghost.PhotographValue;
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPhotographed(destroyedCount);
        }

        StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        _isCharging = true;
        _chargeElapsed = 0f;

        if (_flashlight != null)
        {
            _flashlight.PlayShutterEffect();
        }

        while (_chargeElapsed < _chargeSeconds)
        {
            _chargeElapsed += Time.deltaTime;
            yield return null;
        }

        if (_flashlight != null)
        {
            _flashlight.SetLightActive(true);
        }

        _isCharging = false;
    }
}
