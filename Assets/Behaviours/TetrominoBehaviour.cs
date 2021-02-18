using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TetrominoBehaviour : MonoBehaviour, TetrisEntity
{

    [SerializeField] private float moveDistance;
    [SerializeField] private float fallSpeed;
    
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private CommandController _commandController;

    private Vector2 _nextPosition;
    private float _rotation;
    private bool _movingDown;
    private bool _onGround;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<PolygonCollider2D>();
        _commandController = gameObject.GetComponent<CommandController>();
        
        _nextPosition = _rigidbody.position;
        _rotation = _rigidbody.rotation;
        _movingDown = false;
        _onGround = false;
    }

    private void Update()
    {
        if (_onGround) return;
        // time if its actively being pushed down, the float otherwise
        Descent(_movingDown ? Time.deltaTime : 0.001f);
        _rigidbody.MovePosition(_nextPosition);
        _rigidbody.SetRotation(_rotation);
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
        _rotation += change;
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
