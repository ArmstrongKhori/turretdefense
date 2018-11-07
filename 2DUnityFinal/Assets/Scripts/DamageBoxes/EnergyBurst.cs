using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An electric burst that continuously damages those within.
/// </summary>
public class EnergyBurst : DamageArea
{

    public float fullSize = 5.0f;

    protected override void Awake()
    {
        base.Awake();
        //
        AssignSprite("NASprite"); // ??? <-- This has no graphic at the moment...
    }

    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        durability = 9999; // ??? <-- Doesn't break... But not like this.
        isSustained = true;
        incorporeal = true; // *** Don't break when hitting walls
        power = 15.0f;
        existPeriod = Helper.SECOND;
    }
    public override void Despawn()
    {
        base.Despawn();
        //
        // *** This object does not get pooled (or at least, it shouldn't be)
        Discard();
    }


    public override void Act()
    {
        base.Act();
        //
        cc.radius = fullSize * Helper.Longevity(runTime, 0, existPeriod);
    }

}
