using UnityEngine;

public class MoveCommand : Command
{
    private float _direction;
    private float _moveDistance;
    private int _axis;
    
    public MoveCommand(float time, float direction, float moveDistance, int axis) 
        : base(time)
    {
        _direction = direction;
        _moveDistance = moveDistance;
        _axis = axis;
    }

    public override void Execute(TetrisEntity entity)
    {
        // fallMultiplier is 1 for x axis, or -1 for y axis
        // this is used to ensure that y is always negative
        base.Execute(entity);
        int fallMultiplier = (_axis * -2 + 1);
        Entity.SetNextPosition(_direction * _moveDistance * fallMultiplier, _axis);
    }

    public override void Undo()
    {
        Entity.SetNextPosition(-1f * _direction * _moveDistance, _axis);
    }
}