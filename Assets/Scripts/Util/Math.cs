using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note that this is separate from Mathf.
public class Math {
    // Unity's built-in smoothstep returns a value between [edge0, edge1]
    // However glsl's smoothstep returns a value between [0, 1] which is what
    // this function does.
    public static float Smoothstep(float edge0, float edge1, float x) {
        float t = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        return t * t * (3.0f - 2.0f * t);
    }
}
