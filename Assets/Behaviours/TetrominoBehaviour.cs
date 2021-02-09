using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TetrominoBehaviour : MonoBehaviour
{

    [SerializeField] private float moveDistance;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction)
    {
        var position = _rigidbody.position;
        position.x += direction.x * moveDistance;
        _rigidbody.position = position;
    }

    public void onMove(InputAction.CallbackContext directionContext)
    {
        Move(directionContext.ReadValue<Vector2>());
    }
}
