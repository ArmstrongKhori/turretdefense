using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data about how a Weapon fires, how long it takes, and what it should do when it fires/reloads.
/// This class is meant to be overloaded, but is not necessarily abstract.
/// </summary>
public class WeaponData  {

    /// <summary>
    /// Does this weapon aim ahead of its target...?
    /// </summary>
    public bool predictive = false;
    public float projectedSpeed;
    public float chargeRate;
    /// <summary>
    /// Does this weapon have a recovery period...?
    /// </summary>
    public bool canDischarge = false;
    public float dischargeRate;


    public WeaponData()
    {
        chargeRate = (1.0f * 100) / Helper.SECOND; // *** Takes 1 second to charge up...
        dischargeRate = 100 / Helper.SECOND; // *** Discharge immediately!
    }


    /// <summary>
    /// Triggers every tick that the weapon is charging (the "aiming" process).
    /// </summary>
    /// <param name="s"></param>
    /// <param name="w"></param>
    public virtual void OnCharge(Structure s, Weapon w) { }

    /// <summary>
    /// Triggers every tick that the weapon is discharging (recovering from firing).
    /// </summary>
    /// <param name="s"></param>
    /// <param name="w"></param>
    public virtual void OnDischarge(Structure s, Weapon w) { }

    /// <summary>
    /// Triggers the moment discharging begins (the weapon is fired).
    /// </summary>
    /// <param name="s"></param>
    /// <param name="w"></param>
    public virtual void OnFire(Structure s, Weapon w) { }

    /// <summary>
    /// Triggers the moment charging begins (the weapon is done recovering from fire).
    /// </summary>
    /// <param name="s"></param>
    /// <param name="w"></param>
    public virtual void OnReady(Structure s, Weapon w) { }


    /// <summary>
    /// Called the moment the weapon is created.
    /// Used primarily for creating an object pool so that the weapon
    /// constantly recycles the same bullets.
    /// </summary>
    /// <param name="w"></param>
    public virtual void OnWeaponCreate(Weapon w) { }
}
