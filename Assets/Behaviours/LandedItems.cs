using System;
using System.Linq;
using UnityEngine;

public class LandedItems : MonoBehaviour
{
    [SerializeField] public GameObject Prefab;
    private GameObject[] landedSquares;

    public void addSquares(float[][] positions, string colour)
    {
        for (int s = 0; s < positions.Length; s++)
        {
            landedSquares.Append(Instantiate(Prefab, 
                new Vector2(positions[s][0], positions[s][1]), Quaternion.identity));
        }
    }
}
