using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private const float DirectionStepDegrees = 45f;
    private const float DirectionHalfStepDegrees = 22.5f;
    private const float DirectionHysteresisDegrees = 4f;
    private const float MoveInputSqrMagnitudeThreshold = 0.0001f;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector2 _mapMin = new Vector2(-100f, -60f);
    [SerializeField] private Vector2 _mapMax = new Vector2(100f, 60f);
    [SerializeField] private bool _showMapBoundsGizmo = true;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _moveInput;
    private int _currentDirectionIndex;
    private bool _hasCurrentDirection;
    private string _lastPlayedStateName;

    public float AimAngleDegrees => _currentDirectionIndex * DirectionStepDegrees;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Time.timeScale == 0f)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = -mainCamera.transform.position.z;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude == 0f)
        {
            return;
        }

        float angle = Mathf.Repeat(
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg,
            360f);

        UpdateDirection(angle);
        UpdateAnimation();
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

    private void UpdateDirection(float angle)
    {
        if (_hasCurrentDirection)
        {
            float currentDirectionAngle = _currentDirectionIndex * DirectionStepDegrees;
            float angleFromCurrentDirection = Mathf.Abs(
                Mathf.DeltaAngle(currentDirectionAngle, angle));

            if (angleFromCurrentDirection <=
                DirectionHalfStepDegrees + DirectionHysteresisDegrees)
            {
                return;
            }
        }

        _currentDirectionIndex = Mathf.RoundToInt(angle / DirectionStepDegrees) % 8;
        _hasCurrentDirection = true;

        switch (_currentDirectionIndex)
        {
            case 3:
            case 4:
            case 5:
                _spriteRenderer.flipX = true;
                break;
            default:
                _spriteRenderer.flipX = false;
                break;
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = _moveInput.sqrMagnitude > MoveInputSqrMagnitudeThreshold;
        string statePrefix = isMoving ? "step_dir" : "idle_dir";
        string directionId;

        switch (_currentDirectionIndex)
        {
            case 0:
            case 4:
                directionId = "04";
                break;
            case 1:
            case 3:
                directionId = "06";
                break;
            case 2:
                directionId = "08";
                break;
            case 5:
            case 7:
                directionId = "02";
                break;
            default:
                directionId = "00";
                break;
        }

        string stateName = statePrefix + directionId;

        if (stateName == _lastPlayedStateName)
        {
            return;
        }

        _animator.Play(stateName);
        _lastPlayedStateName = stateName;
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
