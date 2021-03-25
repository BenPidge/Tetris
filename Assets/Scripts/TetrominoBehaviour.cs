using System;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CommandController))]
public class TetrominoBehaviour : MonoBehaviour, TetrisEntity
{
    public static event Action<Vector2[], Sprite> Landed;

    [SerializeField] private float moveDistance;
    // represents the fall speed modifiers for general falling, and falling with user-inputted increased speeds
    [SerializeField] private float defaultFallSpeedMod;
    [SerializeField] private float enhancedFallSpeedMod;

    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private CommandController _commandController;
    private Transform _transform;
    private GameObject _gameObject;

    private RaycastHit2D[] _raycastResults = new RaycastHit2D[10];
    private Vector2 _nextPosition;
    private float _lastFallTime;
    private float _nextRotation;
    private float _fallSpeed;

    private bool _movingDown;
    private bool _onGround;
    private bool _hasMoved;
    private bool _alive;

    private void Awake()
    {
        _gameObject = gameObject;
        _rigidbody = _gameObject.GetComponent<Rigidbody2D>();
        _collider = _gameObject.GetComponent<PolygonCollider2D>();
        _commandController = _gameObject.GetComponent<CommandController>();
        _transform = _gameObject.GetComponent<Transform>();
        _fallSpeed = FindObjectOfType<TetrominoManager>().fallSpeed;

        _nextPosition = _rigidbody.position;
        _lastFallTime = Time.time;
        _alive = true;
        _movingDown = false;
        _onGround = false;
        _hasMoved = false;

        Transform[] children = _gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetParent(_transform, true);
        }
    }

    private void Update()
    {
        if (_onGround) return;
        if (!_alive) return;

        // time if its actively being pushed down, the float otherwise
        Descent(_movingDown ? enhancedFallSpeedMod : defaultFallSpeedMod);
        _hasMoved = false;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_nextPosition);
        _rigidbody.SetRotation(_nextRotation);
    }



    private void Descent(float dt)
    {
        if (Time.time - _lastFallTime >= _fallSpeed / (4 * dt))
        {
            _commandController.ExecuteCommand(new MoveCommand(this, 1, 1, 1));
            _lastFallTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_alive) return;
        // if the collision isn't the sides of two objects touching
        if (Math.Abs(_collider.bounds.min.x - other.bounds.max.x) > 0.01 &&
            Math.Abs(_collider.bounds.max.x - other.bounds.min.x) > 0.01)
        {
            _onGround = true;
            var childRender = GetComponentsInChildren<SpriteRenderer>()[0];
            Landed?.Invoke(GetSquareVectors(), childRender.sprite);

            _alive = false;
            Destroy(_gameObject);
        }
    }

    private Vector2[] GetSquareVectors()
    {
        var vectors = new Vector2[4];
        var x = 0;
        foreach (Transform child in transform)
        {
            vectors[x] = child.position;
            x++;
        }

        return vectors;
    }

    public void SetNextPosition(float newPosition, int direction)
    {
        if (!_hasMoved)
        {
            _nextPosition[direction] += newPosition;
            _hasMoved = true;
        }
    }

    public void SetNextRotate(int num)
    {
        _nextRotation = (_rigidbody.rotation + 90 * num) % 360;
        int calculation = (int) (_nextRotation / 90);
        Vector2 rotationDirection
            = calculation % 2 == 0 ? new Vector2(0, calculation - 1) : new Vector2(2 - calculation, 0);

        var numValidArrays = CheckRotate(rotationDirection, _collider.bounds.max.y);
        numValidArrays += CheckRotate(rotationDirection, _collider.bounds.min.y);

        if (numValidArrays > 0)
        {
            _nextRotation = _rigidbody.rotation;
        }
    }

    private int CheckRotate(Vector2 rotationDirection, float yBounds)
    {
        int numRays = Physics2D.RaycastNonAlloc(_rigidbody.position, rotationDirection, _raycastResults,
            Math.Abs(yBounds - _collider.bounds.center.y));

        // for every ray, subtract one from the number of valid arrays if it's colliding with itself
        int numValidArrays = numRays;
        for (var i = 0; i < numRays; i++)
        {
            RaycastHit2D ray = _raycastResults[i];
            if (ray.transform.IsChildOf(_transform))
            {
                numValidArrays--;
            }
        }
        return numValidArrays;
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
        if (!_onGround && context.performed)
        {
            _commandController.ExecuteCommand(new RotateCommand(this));
        }
    }
}
