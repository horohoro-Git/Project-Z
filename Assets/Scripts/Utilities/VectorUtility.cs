using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtility
{
    public static Vector3 RotateY(Vector3 rotatingVector, float rotateValue)
    {
        Vector3 rotatedVector = Quaternion.Euler(new Vector3(0, rotateValue, 0)) * rotatingVector;
        return rotatedVector;
    }
}
