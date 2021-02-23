public class RotateCommand : Command
{
    private float _rotation;
    
    public RotateCommand(TetrisEntity entity, float rotation) : base(entity)
    {
        _rotation = rotation;
    }

    public override void Execute()
    {
        _entity.SetRotation(_rotation);
    }

    public override void Undo()
    {
        _entity.SetRotation(_rotation * -1f);
    }
}