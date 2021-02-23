public abstract class Command
{
    protected TetrisEntity _entity;

    public Command(TetrisEntity entity)
    {
        _entity = entity;
    }

    public abstract void Execute();
    public abstract void Undo();
}