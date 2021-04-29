using UnityEngine;

public class RotateCommand : Command
{
    public RotateCommand(float time) : base(time)
    {
    }

    public override void Execute(TetrisEntity entity)
    {
        base.Execute(entity);
        Entity.SetNextRotate(1);
    }
}