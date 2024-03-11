using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceTest : MonoBehaviour
{
    public Transform testPosition;

    private void Update()
    {
        Debug.Log(Vector3.Distance(testPosition.position, transform.position));
    }
}
