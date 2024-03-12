using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UIarray : MonoBehaviour
{
    public float amount;
    public Image[] images;

    [Button]
    public void PlaceItems()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (amount <= i) images[i].gameObject.SetActive(false);
            else images[i].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlaceItems();
    }
}
