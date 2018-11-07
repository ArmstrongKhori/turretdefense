using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class describes all the various "things" that can subject Creeps to pain or death.
/// It is inclusive to explosions, bullets, force fields, pools of acid, and so on...
/// As such, it was called "Damage Areas" for lack of a better term...
/// </summary>
public class DamageArea : Actor {

    /// <summary>
    /// Does this ignore obstructions?
    /// </summary>
    public bool incorporeal = false;
    /// <summary>
    /// Is the damage dealt throughout an extended period of time?
    /// </summary>
    public bool isSustained = false;
    public float power = 0.0f;
    public int durability;
    //
    public float existPeriod;


    protected override void Awake()
    {
        base.Awake();
        //
        alive = false;
        gameObject.SetActive(false);
    }

    public override void Despawn()
    {
        // base.Despawn();
        //
        alive = false;
        gameObject.SetActive(false);
    }

    public override void Act()
    {
        base.Act();
        //
        if (runTime >= existPeriod)
        {
            Despawn();
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        // *** If we hit a creep...
        Creep c = collision.gameObject.GetComponent<Creep>();
        if (c != null)
        {
            c.GetShot(this);
        }
        else
        {
            if (!incorporeal)
            {
                Obstruction o = collision.gameObject.GetComponent<Obstruction>();
                if (o != null)
                {
                    // c.GetShot(this);
                    OnMiss(o);
                    //
                    Despawn();
                }
            }
        }
    }


    /// <summary>
    /// Limits the number of creeps this projectile can hit.
    /// This is mainly relevant for "piercing" weapons, which lose
    /// damage and eventually despawn as they pierce targets.
    /// </summary>
    /// <param name="v"></param>
    public void BreakUp(int v)
    {
        durability -= v;
        //
        if (durability <= 0)
        {
            Despawn();
        }
    }


    /// <summary>
    /// Called when colliding with an obstruction.
    /// </summary>
    /// <param name="c"></param>
    public virtual void OnMiss(Obstruction c) { }
    /// <summary>
    /// Called when colliding with a creep.
    /// </summary>
    /// <param name="c"></param>
    public virtual void OnHit(Creep c) { }

    /// <summary>
    /// Called by whatever created it, *usually* upon creation.
    /// </summary>
    /// <param name="by"></param>
    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        power = 10.0f;
        existPeriod = 3 * Helper.SECOND;
        durability = 1;
        runTime = 0;

        alive = true;
        gameObject.SetActive(true);
    }
}
