using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickupBase
{
    float ammoAmount = 25;
    public override void UnlockPickup()
    {
        if(Player.instance.ammo + ammoAmount > Player.instance.maxAmmo)
        {
            Player.instance.ammo = Player.instance.maxAmmo;
        }
        else Player.instance.ammo += ammoAmount;

        FeedbackManager.instance.ammoPickup.Play();

        Destroy(gameObject);
    }
}
