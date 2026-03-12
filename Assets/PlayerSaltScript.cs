using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSaltScript : MonoBehaviour
{
    public Rigidbody2D rigidBody;

    public InputAction jumpAction;
    public InputAction moveAction;

    private bool _jumpRequested;
    private float _jumpImpulse = 30;

    private float _moveInput;
    private float _moveSpeed = 20;
    private float _acceleration = 50;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction = InputSystem.actions.FindAction("Move");

        rigidBody.gravityScale = 5;
        rigidBody.freezeRotation = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _moveInput = moveAction.ReadValue<Vector2>().x;
        Debug.Log($"Move input: {_moveInput}");

        if (jumpAction.WasPressedThisFrame())
        {
            _jumpRequested = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_jumpRequested)
        {
            rigidBody.AddForceY(_jumpImpulse, ForceMode2D.Impulse);
            _jumpRequested = false;
        }

        // Moving horizontally:
        float TargetVelocity = _moveInput * _moveSpeed;
        float VelocityDifference = TargetVelocity - rigidBody.linearVelocity.x;
        float force = VelocityDifference * _acceleration;
        rigidBody.AddForceX(force);
    }
}
