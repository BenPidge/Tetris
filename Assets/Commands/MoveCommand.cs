using UnityEngine;

public class MoveCommand : Command
{
    private float _direction;
    private float _moveDistance;
    private int _axis;
    
    public MoveCommand(TetrisEntity entity, float direction, float moveDistance, int axis) : base(entity)
    {
        _direction = direction;
        _moveDistance = moveDistance;
        _axis = axis;
    }

    public override void Execute()
    {
        // fallMultiplier is 1 for x axis, or -1 for y axis
        // this is used to ensure that y is always negative
        var fallMultiplier = (_axis * -2 + 1);
        _entity.SetNextPosition(_direction * _moveDistance * fallMultiplier, _axis);
    }

    public override void Undo()
    {
        _entity.SetNextPosition(-1f * _direction * _moveDistance, _axis);
    }
}