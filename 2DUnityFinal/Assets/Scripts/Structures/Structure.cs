using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Structures" are the primary defensive tool the player gets for fighting Creeps (the enemies of this game).
/// Structures operate automatically and use built-in "weapons" they have to fight off the creep waves.
/// </summary>
public class Structure : Entity {

    public float acquireThreshold = 10.0f;
    //
    public Creep target = null;
    public Weapon weapon = null;


    /// <summary>
    /// Can this structure be targetted/damaged by Creeps...?
    /// </summary>
    public bool indestructible = true;


    public enum StructureTypes
    {
        Core, // *** The center of the game. If one of these are destroyed, the player loses!
        Pellet, // *** Fires small bullets rapidly.
    }
    /* !!! <-- Future note: "Structure AIs"
     * - Aims for nearest creep
     * - Aims for creep closest to the core
     * - Aims for Red, Blue, then White creeps respectively.
     * */


    public StructureTypes type;
    public void ChangeType(StructureTypes t)
    {
        type = t;
        //
        Database db = Database.Instance();
        switch (type)
        {
            case StructureTypes.Core:
                sightRadius = -1;
                break;
            case StructureTypes.Pellet:
                sightRadius = 10.0f; // ??? <-- 5.0f
                //
                weapon = new Weapon(db.QueryWeaponData("PelletGun"));
                break;
        }
        //
        AssignSprite("Turret_" + type);
    }


    /// <summary>
    /// Targets a Creep.
    /// </summary>
    /// <param name="c"></param>
    public void LockOn(Creep c)
    {
        target = c;
    }
    /// <summary>
    /// Untargets a Creep.
    /// It also calls "Assess()" to quickly find a new target.
    /// </summary>
    public void LockOff()
    {
        target = null;
        //
        Assess(); // *** Turrets immediately assess after breaking targetting, allowing them to quickly find a new target!
    }


    public override void Act()
    {
        if (weapon != null)
        {
            // *** Break targetting if current target is too far!
            // *** This takes utmost priority!
            // ??? <-- ... But could still be optimized...
            if (target != null)
            {
                if (target.isDead)
                {
                    LockOff();
                }
                /*
                else if (Helper.PointDistance(transform.position, target.transform.position) > sightRadius)
                {
                    LockOff();
                }
                */
                else
                {
                    Collider2D col = Helper.LOS(this, target.pos, sightRadius, Helper.LAYER_OBSTRUCT | Helper.LAYER_CREEP);
                    if (col == null || col.gameObject != target.gameObject)
                    {
                        LockOff();
                    }
                    else
                    {
                        Vector2 targettedPos;
                        if (weapon.data.predictive)
                        {
                            // *** Get the distance between you and the target
                            float dist = Helper.PointDistance(transform.position, target.transform.position);
                            // *** Determine the time that would be required for the projectile to reach the target.
                            float eta = dist / weapon.data.projectedSpeed;
                            //
                            // *** Where will the target be after this much time passes...?
                            targettedPos = target.pos + (target.rb.velocity * eta);
                        }
                        else
                        {
                            targettedPos = target.transform.position;
                        }
                        //
                        float dire = Helper.PointDirection(pos, targettedPos);
                        //
                        Direction = Mathf.MoveTowardsAngle(Direction, dire, turnRate / Helper.SECOND);
                        //
                        rb.rotation = Direction;
                        //
                        if (Mathf.DeltaAngle(dire, Direction) < acquireThreshold)
                        {
                            weapon.Charge(this);
                        }
                    }
                }
            }
        }
        //
        base.Act();
    }

    /// <summary>
    /// Structures spend most of their time checking for nearby, visible creeps.
    /// ... Nothing fancy.
    /// </summary>
    public override void Assess()
    {
        base.Assess();
        //
        // *** Only look for a target if you have a weapon to fire at it!
        if (weapon != null)
        {
            // *** Do we need a target?
            if (target == null)
            {
                // *** Check LOS...
                List<Creep> allCreeps = Helper.LOSAllWithinRadius<Creep>(this, sightRadius, Helper.LAYER_OBSTRUCT | Helper.LAYER_CREEP);
                //
                // *** Find the NEAREST creep!
                Creep nearCreep = null;
                foreach (Creep c in allCreeps)
                {
                    if (nearCreep == null)
                    {
                        nearCreep = c;
                    }
                    else if (Helper.PointDistance(transform.position, c.transform.position) < Helper.PointDistance(transform.position, nearCreep.transform.position))
                    {
                        nearCreep = c;
                    }
                }
                //
                // *** ... Now, lock on to it!
                if (nearCreep != null)
                {
                    LockOn(nearCreep);
                }
            }
        }
    }


    /// <summary>
    /// Structures do not actually die. Instead, they "de-power".
    /// This is so that the player can still know where they placed
    /// their turrets after they blow up.
    /// </summary>
    public override void Die()
    {
        if (!isDead)
        {
            lite.intensity *= 0.3f;
            lite.range *= 0.3f;
        }
        //
        base.Die();
        //
        sprite.Visible = true;
    }


    /// <summary>
    /// Called when the structure is attacked by a creep.
    /// This is mainly for the Power Core, which counter-attacks whenever
    /// bitten by a creep.
    /// </summary>
    /// <param name="c"></param>
    public virtual void OnAttacked(Creep c) { }

    
    public int colCount = 0;
    /// <summary>
    /// Used to determine if a structure's placement is applicable.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        colCount++;
    }
    /// <summary>
    /// Used to determine if a structure's placement is applicable.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        colCount--;
    }
}
