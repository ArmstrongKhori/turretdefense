using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (Unimplemented)
/// This weapon fires a piercing shot, but has a slow firing rate.
/// </summary>
public class LaserGun : WeaponData {
    public LaserGun()
    {
        chargeRate = 100 * 1.0f / Helper.SECOND; // *** 1 shot per second
        dischargeRate = 100;
    }


    public override void OnFire(Structure s, Weapon w)
    {

    }
}
