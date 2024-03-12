using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBase : MonoBehaviour
{
    public float pickupDistance = 10f;
    public float unlockDistance = 1f;
    public float lerpSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(Player.instance.transform.position, transform.position) < pickupDistance)
        {
            transform.position = Vector3.Lerp(transform.position, Player.instance.transform.position, lerpSpeed * Time.deltaTime);

            if(Vector3.Distance(Player.instance.transform.position, transform.position) < unlockDistance)
            {
                UnlockPickup();
            }
        }
    }

    public virtual void UnlockPickup()
    {
        Debug.Log("Yea !!");
    }
}
