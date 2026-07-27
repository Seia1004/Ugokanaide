using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class GhostAI : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private Color _stoppedColor = Color.red;

    public bool IsLit { get; private set; }

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Color _normalColor;

    public void Initialize(Transform target, FlashlightController flashlight)
    {
        _target = target;
        _flashlight = flashlight;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _normalColor = _spriteRenderer.color;
    }

    private void FixedUpdate()
    {
        IsLit = _flashlight != null && _flashlight.IsPositionLit(_rigidbody.position);

        if (IsLit)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _spriteRenderer.color = _stoppedColor;
            return;
        }

        _spriteRenderer.color = _normalColor;

        if (_target == null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)_target.position - _rigidbody.position).normalized;
        _rigidbody.linearVelocity = direction * _moveSpeed;
    }
}
