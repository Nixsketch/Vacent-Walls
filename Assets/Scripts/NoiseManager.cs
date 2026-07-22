using System;
using UnityEngine;

public static class NoiseManager
{
    // (position, strength, source)
    public static event Action<Vector3, float, GameObject> OnNoiseEmitted;

    public static void EmitNoise(Vector3 position, float strength, GameObject source = null)
    {
        if (strength <= 0f) return;
        OnNoiseEmitted?.Invoke(position, strength, source);
    }
}
