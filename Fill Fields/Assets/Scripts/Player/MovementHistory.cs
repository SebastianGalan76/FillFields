using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHistory : ScriptableObject
{
    public int[] newBlockPos;
    public int indexPos;

    private MovementDirection direction;
    private int distance;

    public void Initialize(MovementDirection direction, int distance)
    {
        Direction = direction;
        Distance = distance;

        newBlockPos = new int[distance];
    }
    
    public bool CheckNewBlock(int posPlatform)
    {
        for(int i = 0; i < newBlockPos.Length; i++)
        {
            if (posPlatform == newBlockPos[i])
            {
                return true;
            }
        }
        return false;
    }

    public MovementDirection Direction
    {
        get { return direction; }
        set { direction = value; }
    }
    public int Distance
    {
        get { return distance; }
        set { distance = value; }
    }
}
