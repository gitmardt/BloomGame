using System.Collections.Generic;
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

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
