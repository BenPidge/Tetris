public class RotateCommand : Command
{
    public RotateCommand(TetrisEntity entity) : base(entity)
    {
    }

    public override void Execute()
    {
        _entity.IncrementRotates(1);
    }

    public override void Undo()
    {
        _entity.IncrementRotates(-1);
    }
}