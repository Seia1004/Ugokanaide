using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class GhostAI : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private FlashlightController _flashlight;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private bool _defaultFacesRight = true;
    [SerializeField] private float _deathAnimationSeconds = 0.5f;

    public bool IsLit { get; private set; }

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _isDead;

    public void Initialize(Transform target, FlashlightController flashlight)
    {
        _target = target;
        _flashlight = flashlight;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }

        IsLit = _flashlight != null && _flashlight.IsPositionLit(_rigidbody.position);
        _animator.SetBool("IsLit", IsLit);

        if (_target != null && !IsLit)
        {
            float relativeX = _target.position.x - _rigidbody.position.x;

            if (!Mathf.Approximately(relativeX, 0f))
            {
                _spriteRenderer.flipX = _defaultFacesRight ? relativeX < 0f : relativeX > 0f;
            }
        }

        if (IsLit)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        if (_target == null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)_target.position - _rigidbody.position).normalized;
        _rigidbody.linearVelocity = direction * _moveSpeed;
    }

    public void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        IsLit = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Died");
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(_deathAnimationSeconds);
        Destroy(gameObject);
    }
}
