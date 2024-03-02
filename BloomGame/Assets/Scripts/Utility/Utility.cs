using UnityEngine;

public static class Utility
{
    public static Vector3 RandomVector3()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
    }

    public static float Remap(float valueInput, float oldRangeMin, float oldRangeMax, float newRangeMin, float newRangeMax)
    {
        return newRangeMin + (valueInput - oldRangeMin) * (newRangeMax - newRangeMin) / (oldRangeMax - oldRangeMin);
    }

    public static bool IsValid(Vector3 value)
    {
        return !float.IsNaN(value.x) && !float.IsNaN(value.y) && !float.IsNaN(value.z);
    }
}
