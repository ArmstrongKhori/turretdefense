using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple projectile that travels in a direction at a speed.
/// </summary>
public class Pellet : DamageArea {

    TrailRenderer tr;
    protected override void Awake()
    {
        base.Awake();
        //
        tr = gameObject.AddComponent<TrailRenderer>();
        tr.material = (Material)Resources.Load("trails");
        tr.startColor = Color.yellow;
        tr.endColor = new Color(1.0f, 1.0f, 0.0f, 0.0f);
        tr.time = 0.1f;
        tr.widthCurve = AnimationCurve.Constant(0.0f, 1.0f, 3.0f / 256.0f);

        AssignSprite("Blank");


        cc.radius = 0.1f;
    }

    public override void Act()
    {
        base.Act();
        //
        // cc.radius = 2 * Speed / Helper.SECOND; // *** Prevents the bullet from passing through colliders.
    }

    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        tr.Clear();
        power = 10.0f;
        existPeriod = 2.5f * Helper.SECOND;

        Sounder.Instance().PlaySound("pellet_shoot");
    }

    public override void OnMiss(Obstruction c)
    {
        base.OnMiss(c);
        //
        Sounder.Instance().PlaySound("pellet_hit_wall");
    }

    public override void OnHit(Creep c)
    {
        base.OnHit(c);
        //
        Sounder.Instance().PlaySound("pellet_hit_creep");
    }

}
