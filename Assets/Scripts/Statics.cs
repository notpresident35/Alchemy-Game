using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    // Returns a random vector with values ranging from -1 to 1
    public static Vector3 RandVectorPosNeg () {
        return new Vector3 (Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }

    // Returns a random vector with values ranging from 0 to 1
    public static Vector3 RandVectorPos () {
        return new Vector3 (Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
    }
}
