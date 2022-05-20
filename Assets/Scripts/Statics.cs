using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    // Returns a random vector with values ranging from -1 to 1
    public static Vector3 RandVectorPosNeg () {
        return new Vector3 (Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    // Returns a random vector with values ranging from 0 to 1
    public static Vector3 RandVectorPos () {
        return new Vector3 (Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
}
