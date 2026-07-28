using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector2 _mapMin = new Vector2(-100f, -60f);
    [SerializeField] private Vector2 _mapMax = new Vector2(100f, 60f);
    [SerializeField] private bool _showMapBoundsGizmo = true;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = _moveInput * _moveSpeed;
        Vector2 currentPosition = _rigidbody.position;

        if ((currentPosition.x <= _mapMin.x && velocity.x < 0f) ||
            (currentPosition.x >= _mapMax.x && velocity.x > 0f))
        {
            velocity.x = 0f;
        }

        if ((currentPosition.y <= _mapMin.y && velocity.y < 0f) ||
            (currentPosition.y >= _mapMax.y && velocity.y > 0f))
        {
            velocity.y = 0f;
        }

        _rigidbody.linearVelocity = velocity;

        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(currentPosition.x, _mapMin.x, _mapMax.x),
            Mathf.Clamp(currentPosition.y, _mapMin.y, _mapMax.y));

        if (clampedPosition != currentPosition)
        {
            _rigidbody.position = clampedPosition;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_showMapBoundsGizmo)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (_mapMin.x + _mapMax.x) / 2f,
            (_mapMin.y + _mapMax.y) / 2f,
            0f);
        Vector3 size = new Vector3(
            _mapMax.x - _mapMin.x,
            _mapMax.y - _mapMin.y,
            0f);

        Gizmos.DrawWireCube(center, size);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost") && GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}
