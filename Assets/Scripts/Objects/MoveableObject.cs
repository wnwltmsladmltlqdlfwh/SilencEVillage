using System.Numerics;
using UnityEngine;

public class MoveableObject : ObjectBase
{
    protected Area currentArea;
    protected Area targetArea;

    public virtual void MoveToTargetArea()
    {
        if(currentArea == targetArea)
            return;
        currentArea = targetArea;
    }
}