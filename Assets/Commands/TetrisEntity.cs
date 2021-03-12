using UnityEngine;

public interface TetrisEntity
{
    void SetNextPosition(float nextPosition, int direction);
    void SetNextRotate(int num);
    
}