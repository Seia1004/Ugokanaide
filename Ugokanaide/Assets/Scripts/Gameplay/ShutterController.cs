using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShutterController : MonoBehaviour
{
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _chargeSeconds = 3f;

    private bool _isCharging;

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

        GhostAI[] ghosts = FindObjectsByType<GhostAI>(FindObjectsSortMode.None);
        int destroyedCount = 0;

        foreach (GhostAI ghost in ghosts)
        {
            if (ghost.IsLit)
            {
                Destroy(ghost.gameObject);
                destroyedCount++;
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

        if (_flashlight != null)
        {
            _flashlight.PlayShutterEffect();
        }

        yield return new WaitForSeconds(_chargeSeconds);

        if (_flashlight != null)
        {
            _flashlight.SetLightActive(true);
        }

        _isCharging = false;
    }
}
