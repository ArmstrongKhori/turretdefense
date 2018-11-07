using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by Structures.
/// This is what allows them to "operate" guns/weapons.
/// </summary>
public class Weapon {

    public float charge;
    public bool isDischarging = true; // *** So that we get that satisfying "click" of a weapon readying!

    private int weaponIndex = 0;
    public List<DamageArea> weaponCache;
    /// <summary>
    /// Passes out and "spawns" a DamageArea instance.
    /// Essentially Object Pooling code.
    /// </summary>
    /// <returns>Returns a DamageArea</returns>
    public DamageArea Supply()
    {
        DamageArea da = weaponCache[weaponIndex];
        //
        weaponIndex = (weaponIndex + 1) % weaponCache.Count;
        //
        da.OnSpawn();
        //
        //
        return da;
    }

    public readonly WeaponData data;

    public Weapon(WeaponData wd)
    {
        weaponCache = new List<DamageArea>();


        data = wd;
        //
        wd.OnWeaponCreate(this);
    }


    /// <summary>
    /// Called every step, this is what sets up timing for when and how shots are fired.
    /// </summary>
    /// <param name="s"></param>
	public void Charge(Structure s)
    {
        if (isDischarging)
        {
            charge -= data.dischargeRate;
            //
            data.OnDischarge(s, this);
            //
            if (charge <= 0)
            {
                Ready(s);
            }
        }
        else
        {
            charge += data.chargeRate;
            //
            data.OnCharge(s, this);
            //
            if (charge >= 100)
            {
                Fire(s);
            }
        }
    }

    /// <summary>
    /// Called the moment a weapon is done firing.
    /// </summary>
    /// <param name="s"></param>
    public void Ready(Structure s)
    {
        isDischarging = false;
        //
        data.OnReady(s, this);
    }
    /// <summary>
    /// Called the moment a weapon is fired.
    /// </summary>
    /// <param name="s"></param>
    public void Fire(Structure s)
    {
        if (data.canDischarge)
        {
            isDischarging = true;
        }
        else
        {
            charge -= 100;
        }
        
        //
        data.OnFire(s, this);
    }
}
