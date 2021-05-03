using System;
using UnityEngine;

public class TransformCommand : Command
{
    public static event Action<int> Transformed;
    private readonly int _cost;
    
    public TransformCommand(float time, int cost) : base(time)
    {
        _cost = cost;
    }
    
    public override void Execute(TetrisEntity entity)
    {
        base.Execute(entity);
        Transformed?.Invoke(_cost);
    }
}
