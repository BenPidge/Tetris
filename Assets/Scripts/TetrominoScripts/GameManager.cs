using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour
{
    [SerializeField] protected float highestY;
    [SerializeField] public Vector2 spawnPoint;
    [SerializeField] protected List<GameObject> prefabs;
    [SerializeField] protected GameObject specialPrefab;
    [SerializeField] protected GameObject gameOverPanel;
    
    public GameObject currentTetromino;
    protected LandedItems LandedItems;
    public abstract void TransformTetromino();
}
