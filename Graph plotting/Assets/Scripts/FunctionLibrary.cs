using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate float Function(float x, float z, float t);
    
    public enum FunctionName { Wave, MultiWave, Ripple }

    private static readonly Function[] Functions = {Wave, MultiWave, Ripple};

    public static Function GetFunction(FunctionName name)
    {
        return Functions[(int)name];
    }

    private static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }

    private static float MultiWave(float x, float z, float t)
    {
        float y = Sin(PI * (x + 0.5f * t));
        // Division requires a bit more work than multiplication, so it's a rule of thumb to prefer multiplication over
        // division. However, constant expressions like 1f / 2f and also 2f * Mathf.PI are already reduced to a single
        // number by the compiler.
        // y += Sin(2f * PI * (x + t)) / 2f; 
        y += 0.5f * Sin(2f * PI * (z + t));
        y += Sin(PI * (x + z + 0.25f * t));
        return y * (1f / 2.5f);
    }

    private static float Ripple(float x, float z, float t)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10f * d);
    }
    
}