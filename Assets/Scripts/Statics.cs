using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    // Returns a random vector with values ranging from -range to range
    public static Vector3 RandVectorPosNeg (float range) {
        return new Vector3 (Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * range;
    }

    // Returns a random vector with values ranging from 0 to range
    public static Vector3 RandVectorPos (float range) {
        return new Vector3 (Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1)) * range;
    }
}
