using UnityEngine;

public abstract class Command
{
    protected TetrisEntity Entity;
    public float TimeExecuted;

    public Command(float time)
    {
        TimeExecuted = time;
    }

    public virtual void Execute(TetrisEntity entity)
    {
        Entity = entity;
    }
    public abstract void Undo();
}