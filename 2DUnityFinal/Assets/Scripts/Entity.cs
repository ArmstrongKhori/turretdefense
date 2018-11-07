using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An actor that can live, think, and die.
/// </summary>
public class Entity : Actor {

    public float maxHealth = 100;
    public float health = 100;
    public bool isDead = false;

    private float reassessTime;
    public float reassessRate;
    public float sightRadius;
    //
    public float turnRate = 180.0f;

    public Light lite;


    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        //
        reassessRate = Helper.SECOND;
        sightRadius = 1.0f;


        lite = gameObject.AddComponent<Light>();
        lite.intensity = 3;
        lite.range = 1;
        lite.type = LightType.Point;
        lite.enabled = false;
    }

    /// <summary>
    /// Turns on the entity's light.
    /// </summary>
    public void LitesOn()
    {
        lite.enabled = true;
    }

    public int highlightTime = 0;
    /// <summary>
    /// Changes the light's color.
    /// Also allows the light to flicker/flash by resetting the "highlightTime".
    /// </summary>
    /// <param name="col"></param>
    /// <param name="doFlash"></param>
    public virtual void Highlight(Color col, bool doFlash = true)
    {
        lite.color = new Color(col.r, col.g, col.b, 1.0f);
        //
        if (doFlash)
        {
            highlightTime = 0;
        }
    }


    public override void Act()
    {
        base.Act();
        //
        float jist = Helper.Longevity(highlightTime, Helper.SECOND, Helper.SECOND / 3);
        highlightTime += 1;
        lite.color = new Color(lite.color.r, lite.color.g, lite.color.b, 1.0f * (1 - jist));
        // lite.intensity
        //
        // *** Reassess! Come up with a new choice!
        reassessTime += 1;
        if (reassessTime >= reassessRate)
        {
            reassessTime -= reassessRate;
            //
            Assess();
        }
    }

    /// <summary>
    /// This is what the entity does at certain intervals.
    /// Runs INTERMITTENTLY, rather than CONSTANTLY.
    /// Used mainly for "updating" behavior and whatnot.
    /// </summary>
    public virtual void Assess()
    {
        // *** ...?
    }

    /// <summary>
    /// Get hurt.
    /// When your health runs out, you "die()".
    /// </summary>
    /// <param name="val"></param>
    public void TakeDamage(float val)
    {
        health -= val;
        //
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Makes the entity "die".
    /// What this means can vary between each entity.
    /// </summary>
    public virtual void Die()
    {
        isDead = true;
        //
        // Despawn();
        alive = false;
        sprite.Visible = false;
    }

    /// <summary>
    /// Happens at the moment the entity is created.
    /// For entities, it turns on their lights.
    /// </summary>
    /// <param name="by"></param>
    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        LitesOn();
    }
}
