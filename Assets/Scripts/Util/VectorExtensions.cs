using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions {
    public static Vector3 getHorizontalPart(this Vector3 v) {
        return new Vector3(v.x, 0.0f, v.z);
    }

    public static Vector3 getVerticalPart(this Vector3 v) {
        return new Vector3(0.0f, v.y, 0.0f);
    }
}
