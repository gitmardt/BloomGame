using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class ObjectArrayCounter : MonoBehaviour
{
    public float amount;
    public GameObject[] objs;

    [Button]
    public void PlaceItems()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            if (amount <= i) objs[i].SetActive(false);
            else objs[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlaceItems();
    }
}
