using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CommandController))]
public class TetrominoBehaviour : MonoBehaviour, TetrisEntity
{
    public static event Action<Vector2[], Sprite> Landed;
    
    [SerializeField] private float moveDistance;
    [SerializeField] private float fallSpeed;
    
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private CommandController _commandController;

    private Vector2 _nextPosition;
    private int _numOfRotatePresses;
    private bool _movingDown;
    private bool _onGround;
    private bool _hasMoved;

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<PolygonCollider2D>();
        _commandController = gameObject.GetComponent<CommandController>();
        
        _nextPosition = _rigidbody.position;
        _movingDown = false;
        _onGround = false;
        _hasMoved = false;
        _numOfRotatePresses = 0;

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

        if (_numOfRotatePresses > 0)
        {
            _rigidbody.SetRotation((float) Math.Round(_rigidbody.rotation + (90 * _numOfRotatePresses)));
            _numOfRotatePresses = 0;
        }
        _hasMoved = false;
    }

    private void Descent(float dt)
    {
        _commandController.ExecuteCommand(new MoveCommand(this, fallSpeed, dt, 1));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Math.Abs(_collider.bounds.min.x - other.bounds.max.x) > 0.01 &&
            Math.Abs(_collider.bounds.max.x - other.bounds.min.x) > 0.01)
        {
            _onGround = true;
            var childRender = GetComponentsInChildren<SpriteRenderer>()[0];
            Landed?.Invoke(GetSquareVectors(), childRender.sprite);
            Destroy(gameObject);
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

    public void IncrementRotates(int num)
    {
        _numOfRotatePresses += num;
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
            _commandController.ExecuteCommand(new RotateCommand(this));
        }
    }
}
