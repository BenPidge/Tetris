using System;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TetrominoBehaviour : MonoBehaviour, TetrisEntity
{
    public static event Action<Vector2[], Sprite> Landed;

    [SerializeField] private float moveDistance;
    // represents the fall speed modifiers for general falling, and falling with user-inputted increased speeds
    [SerializeField] private float defaultFallSpeedMod;
    [SerializeField] private float enhancedFallSpeedMod;

    public Transform[] children;
    public Rigidbody2D wholeRigidbody;
    private PolygonCollider2D _collider;
    private Transform _transform;
    private GameObject _gameObject;
    private SpriteRenderer _childRender;
    private TetrominoManager _manager;

    private readonly RaycastHit2D[] _rotateRaycastResults = new RaycastHit2D[10];
    private readonly RaycastHit2D[] _moveRaycastResults = new RaycastHit2D[10];
    private Vector2 _nextPosition;
    private float _lastFallTime;
    private float _nextRotation;

    private bool _movingDown;
    private bool _onGround;
    private bool _hasMoved;
    private bool _alive;
    private bool _replayTetromino;

    private void Awake()
    {
        _gameObject = gameObject;
        wholeRigidbody = _gameObject.GetComponent<Rigidbody2D>();
        _collider = _gameObject.GetComponent<PolygonCollider2D>();
        _transform = _gameObject.GetComponent<Transform>();
        _childRender = GetComponentsInChildren<SpriteRenderer>()[0];
        _manager = FindObjectOfType<TetrominoManager>();

        _nextPosition = wholeRigidbody.position;
        _lastFallTime = Time.time;
        _alive = true;
        _movingDown = false;
        _onGround = false;
        _hasMoved = false;
        _replayTetromino = false;

        children = _gameObject.GetComponentsInChildren<Transform>();
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
        if (!_replayTetromino)
        {
            Descent(_movingDown ? enhancedFallSpeedMod : defaultFallSpeedMod);
        }
        CheckHasLanded();
        wholeRigidbody.MovePosition(_nextPosition);
        wholeRigidbody.SetRotation(_nextRotation);
        _hasMoved = false;
    }



    private void Descent(float dt)
    {
        if (Time.time - _lastFallTime >= _manager.fallSpeed / dt)
        {
            CommandController.ExecuteCommand(new MoveCommand(Time.timeSinceLevelLoad, 1, 1, 1));
            _lastFallTime = Time.time;
        }
    }

    private void CheckHasLanded()
    {
        if (!_alive) return;
        // if the collision isn't the sides of two objects touching
        if (!CheckMove(Vector2.down) && _nextPosition.y <= wholeRigidbody.position.y)
        {
            _onGround = true;
            Vector2[] squareVectors = GetSquareVectors();

            _alive = false;
            Destroy(_gameObject);
            Landed?.Invoke(squareVectors, _childRender.sprite);
        }
    }

    private Vector2[] GetSquareVectors()
    {
        Vector2[] vectors = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            vectors[i] = transform.GetChild(i).position;
        }

        return vectors;
    }

    public void SetReplay(bool isReplay)
    {
        _replayTetromino = isReplay;
    }
    
    public void SetNextPosition(float newPosition, int direction)
    {
        Vector2 dir = new Vector2(0, 0);
        dir[direction] = newPosition;
        if (!_hasMoved && CheckMove(dir))
        {
            _nextPosition[direction] += newPosition;
            _hasMoved = true;
        }
    }

    public void SetNextRotate(int num)
    {
        float rotation = wholeRigidbody.rotation;
        _nextRotation = CheckRotate(num) == 0 ? (rotation + 90 * num) % 360 : rotation;
    }

    private int CheckRotate(int rotateDir)
    {
        int rayHits;
        int numValidRays = 0;
        Vector2 localPos;
        Vector2 rotationDirection;

        // for every square in the tetromino, shoot off rays
        for (int i = 0; i < children.Length; i++)
        {
            localPos = children[i].position - _collider.transform.position;
            rotationDirection = new Vector2(-(localPos.x + localPos.y), (localPos.x - localPos.y)*rotateDir);
            
            Array.Clear(_rotateRaycastResults, 0, _rotateRaycastResults.Length);
            rayHits = Physics2D.RaycastNonAlloc(children[i].position, rotationDirection, 
                _rotateRaycastResults, 1);
            Debug.DrawRay(children[i].position, rotationDirection, Color.blue, 1);
        
            numValidRays += rayHits;
            // for every ray, subtract one from the number of valid arrays if it's colliding with itself
            for (int j = 0; j < rayHits; j++)
            {
                if (_rotateRaycastResults[j].transform.IsChildOf(_transform))
                {
                    numValidRays--;
                }
            }
        }
        
        return numValidRays;
    }

    private bool CheckMove(Vector2 direction)
    {
        int rayHits;
        int validRayHits = 0;
        
        // for every square in the tetromino, shoot off rays
        for (int i = 0; i < children.Length; i++)
        {
            Array.Clear(_moveRaycastResults, 0, _moveRaycastResults.Length);
            rayHits = Physics2D.RaycastNonAlloc(children[i].position, direction, 
                _moveRaycastResults, 1);
            
            validRayHits += rayHits;
            // for every ray, subtract one from the number of valid arrays if it's colliding with itself
            for (int j = 0; j < rayHits; j++)
            {
                if (_moveRaycastResults[j].transform.IsChildOf(_transform))
                {
                    validRayHits--;
                }
            }
        }

        return validRayHits == 0;
    }
    
// Input-called methods
    public void OnMove(InputAction.CallbackContext directionContext)
    {
        if (_onGround) return;
        Vector2 direction = directionContext.ReadValue<Vector2>();
        if (direction.y < 0)
        {
            _movingDown = true;
        }
        else if (!_replayTetromino)
        {
            CommandController.ExecuteCommand(new MoveCommand(Time.timeSinceLevelLoad, direction.x, moveDistance, 0));   
        }
    }
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!_onGround && context.performed && !_replayTetromino)
        {
            CommandController.ExecuteCommand(new RotateCommand(Time.timeSinceLevelLoad));
        }
    }
}
