using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This weapon fires simple bullets at a steady rate.
/// This is the "balancing" weapon that all others weapons follow close to.
/// </summary>
public class PelletGun : WeaponData
{
    

    public PelletGun() : base()
    {
        predictive = true;
        projectedSpeed = 12.0f;
        //
        chargeRate = (2.5f * 100) / Helper.SECOND; // *** Two and a half shots per second
        dischargeRate = 100;
    }

    public override void OnWeaponCreate(Weapon w)
    {
        base.OnWeaponCreate(w);
        //
        Director d = Director.Instance();

        int projCount = 1 + Mathf.CeilToInt(chargeRate / 100 * Helper.SECOND);
        w.weaponCache.Clear();
        for (int i = 0; i < projCount; i++)
        {
            w.weaponCache.Add(d.CreateDamageArea<Pellet>(Helper.DEADZONE));
        }
    }


    public override void OnFire(Structure s, Weapon w)
    {
        base.OnFire(s, w);
        //
        Director d = Director.Instance();
        //
        DamageArea Q = w.Supply();
        Q.transform.position = s.transform.position;
        //
        Q.Direction = s.Direction;
        Q.Speed = projectedSpeed;
        //
        
    }
}
