using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSaltScript : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public CircleCollider2D circleCollider;

    public InputAction jumpAction;
    public InputAction moveAction;
    public InputAction debugMoveAction;

    private bool _jumpRequested;
    private bool _jumpCutRequested;
    [SerializeField] private float _jumpImpulse = 30;

    private float _moveInput;
    [SerializeField] private float _maxMoveSpeed = 20;
    [SerializeField] private float _acceleration = 5;

    private float _surfaceCheckCastDistance = 0.1f;
    private Transform _surfaceCheckCast;
    private bool _isOnSurface;

    private bool _debugMoveRequested;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");
        debugMoveAction = InputSystem.actions.FindAction("DebugMove");

        rigidBody.gravityScale = 5;
        rigidBody.freezeRotation = true;

        GameObject gameObject = new GameObject("SurfaceCheck");
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition =  new Vector2(0f, -_surfaceCheckCastDistance);
        _surfaceCheckCast = gameObject.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Updating input values:
        _moveInput = moveAction.ReadValue<Vector2>().x;
        if (jumpAction.WasPressedThisFrame())
        {
            _jumpRequested = true;
        }
        else if (jumpAction.WasReleasedThisFrame())
        {
            _jumpCutRequested = true;
        }
        _debugMoveRequested = debugMoveAction.IsPressed();

        Debug.Log($"transform.position: {transform.position}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DoMovement(true);
    }

    void OnDrawGizmos()
    {
        if (_surfaceCheckCast == null) return;
        Gizmos.color = _isOnSurface ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_surfaceCheckCast.position, circleCollider.radius);
    }

    #region Heper Methods
    // Movement:
    private void DoMovement(bool doDebug)
    {
        DoHorizontalMovement();
        UpdateIsOnSurface();
        TryJumpIfRequested();
        TryJumpCutIfRequested();

        if (doDebug)
            TryDebugMove();
    }

    // Debug movement

    private void TryDebugMove()
    {
        if (_debugMoveRequested)
        {
            DebugMove();
        }
    }

    private void DebugMove()
    {
        var mousePositionScreen = Mouse.current.position.ReadValue();
        transform.position = Camera.main.ScreenToWorldPoint(mousePositionScreen);
    }

    // Moving horizontally:
    private void DoHorizontalMovement()
    {
        float TargetVelocity = _moveInput * _maxMoveSpeed;
        float VelocityDifference = TargetVelocity - rigidBody.linearVelocity.x;
        float force = VelocityDifference * _acceleration;
        rigidBody.AddForceX(force);
    }

    // Jumping:
    private void TryJumpIfRequested()
    {
        if (_jumpRequested & _isOnSurface)
        {
            Jump();
            _jumpRequested = false;
        }
    }
    private void UpdateIsOnSurface()
    {
        Collider2D[] circleCastHits = Physics2D.OverlapCircleAll(_surfaceCheckCast.position, circleCollider.radius);
        foreach (Collider2D hit in circleCastHits)
        {
            if (hit == circleCollider) continue; // Ignore self

            _isOnSurface = true;
            return;
        }
        _isOnSurface = false;

    }
    private void Jump()
    {
        rigidBody.AddForceY(_jumpImpulse, ForceMode2D.Impulse);
    }

    // Jump cutting:
    private void TryJumpCutIfRequested()
    {
        if (_jumpCutRequested)
        {
            if (CanCutJump())
            {
                CutJump();
            }
            _jumpCutRequested = false;
        }
    }
    private bool CanCutJump()
    {
        return rigidBody.linearVelocity.y > 0;
    }
    private void CutJump()
    {
        rigidBody.AddForceY(-rigidBody.linearVelocity.y * 0.5f, ForceMode2D.Impulse);
    }
    #endregion
}
