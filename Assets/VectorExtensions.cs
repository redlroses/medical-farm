using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vector, float x) => new(x, vector.y, vector.z);
    public static Vector3 WithY(this Vector3 vector, float y) => new(vector.x, y, vector.z);
    public static Vector3 WithZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);

    public static Vector2 WithX2(this Vector3 vector, float x) => new(x, vector.y);
    public static Vector2 WithY2(this Vector3 vector, float y) => new(vector.x, y);
}