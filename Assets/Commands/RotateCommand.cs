public class RotateCommand : Command
{
    public RotateCommand(TetrisEntity entity) : base(entity)
    {
    }

    public override void Execute()
    {
        _entity.SetNextRotate(1);
    }

    public override void Undo()
    {
        _entity.SetNextRotate(-1);
    }
}