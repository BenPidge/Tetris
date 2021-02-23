using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CommandController))]
public class TetrominoBehaviour : MonoBehaviour, TetrisEntity
{

    [SerializeField] private float moveDistance;
    [SerializeField] private float fallSpeed;
    
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private CommandController _commandController;

    private Vector2 _nextPosition;
    private bool _movingDown;
    private bool _onGround;
    private bool _rotate;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<PolygonCollider2D>();
        _commandController = gameObject.GetComponent<CommandController>();
        
        _nextPosition = _rigidbody.position;
        _movingDown = false;
        _onGround = false;
        _rotate = false;

        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            child.SetParent(gameObject.transform, true);
        }
    }

    private void Update()
    {
        if (_onGround) return;
        // time if its actively being pushed down, the float otherwise
        Descent(_movingDown ? Time.deltaTime : 0.001f);
        _rigidbody.MovePosition(_nextPosition);
        
        if (_rotate)
        {
            _rigidbody.SetRotation(_rigidbody.rotation + 45);
            _rotate = false;
        }
    }

    private void Descent(float dt)
    {
        _commandController.ExecuteCommand(new MoveCommand(this, fallSpeed, dt, 1));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onGround = true;
    }

    public void SetNextPosition(float newPosition, int direction)
    {
        _nextPosition[direction] += newPosition;
    }

    public void SetRotation(float change)
    {
        _rotate = true;
    }

    // Input-called methods
    public void OnMove(InputAction.CallbackContext directionContext)
    {
        if (_onGround) return;
        var direction = directionContext.ReadValue<Vector2>();
        _movingDown = (direction.y < 0);
        if (direction.y < 0)
        {
            _movingDown = true;
        }
        else
        {
            _commandController.ExecuteCommand(new MoveCommand(this, direction.x, moveDistance, 0));   
        }
    }
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!_onGround)
        {
            _commandController.ExecuteCommand(new RotateCommand(this, 90));
        }
    }
}
