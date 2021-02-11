using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TetrominoBehaviour : MonoBehaviour
{

    [SerializeField] private float moveDistance;
    [SerializeField] private float fallSpeed;
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private bool _movingDown;
    private bool _onGround;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<PolygonCollider2D>();
        _movingDown = false;
        _onGround = false;
    }

    private void FixedUpdate()
    {
        if (_movingDown && !_onGround)
        {
            Descent(Time.deltaTime);
        }
    }

    private void Move(Vector2 direction)
    {
        var position = _rigidbody.position;
        position.x += direction.x * moveDistance;
        _rigidbody.MovePosition(position);
    }

    private void Descent(float dt)
    {
        var position = _rigidbody.position;
        position.y -= fallSpeed * dt;
        _rigidbody.MovePosition(position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.bounds.max.y >= _collider.bounds.min.y)
        {
            _onGround = true;   
        }
    }


    // Input-called methods
    public void OnMove(InputAction.CallbackContext directionContext)
    {
        Vector2 direction = directionContext.ReadValue<Vector2>();
        _movingDown = (direction.y < 0);
        Move(direction);
    }
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        _rigidbody.rotation += 90;
    }
}
