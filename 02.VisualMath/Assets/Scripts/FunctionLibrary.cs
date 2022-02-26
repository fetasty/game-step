using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public delegate Vector3 Function(float u, float v, float t);
public static class FunctionLibrary
{
    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, DisturbSphere, Torus }
    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, DisturbSphere, Torus };
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }
    public static FunctionName GetNextFunctionName(FunctionName name)
    {
        return (int)name < functions.Length - 1 ? name + 1 : 0;
    }
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p = Vector3.zero;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p = Vector3.zero;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += Sin(2f * PI * (v + t)) * (1f / 2f);
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= (1f / 2.5f);
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        Vector3 p = Vector3.zero;
        p.x = u;
        p.z = v;
        float d = Sqrt(u * u + v * v);
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p = Vector3.zero;
        float r = Cos(0.5f * PI * v);
        p.x = r * Cos(PI * u + t);
        p.z = r * Sin(PI * u + t);
        p.y = Sin(0.5f * PI * v);
        return p;
    }

    public static Vector3 DisturbSphere(float u, float v, float t)
    {
        Vector3 p = Vector3.zero;
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        p.x = s * Sin(PI * u);
        p.z = s * Cos(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p = Vector3.zero;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Morph(
        float u, float v, float t, Function from, Function to, float progress
    )
    {
        // return Vector3.Lerp(from(u, v, t), to(u, v, t), progress);
        return Vector3.LerpUnclamped(
            from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress)
        );
    }
}
