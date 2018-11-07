using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The primary "enemy" entity in the game.
/// These will home towards the core and destroy it if they get too close.
/// Structures will launch attacks against these to destroy and supress their movements.
/// </summary>
public class Creep : Entity {

    public enum States
    {
        Inactive, // *** Doesn't move; just spawned in and can't be hit
        Moving, // *** Moves.
        Waiting,
        Attacking,
    }
    public States state = States.Inactive;
    public void ChangeState(States s)
    {
        switch (s)
        {
            case States.Inactive:
                LitesOn();
                break;
        }
        //
        state = s;
        //
        switch (s)
        {
            case States.Moving:
                Speed = 0.25f;
                break;

            case States.Waiting:
                Speed = 0.0f;
                break;

            case States.Attacking:
                Speed = 0.0f;
                break;
        }
    }


    public float power = 5.0f;


    public bool awareOfCore = false;
    public bool curious = false;
    Vector2 destinationPos;
    //
    public float waitTime = 0;
    /// <summary>
    /// Tells the Creep to become inactive for "howLong"
    /// </summary>
    /// <param name="howLong"></param>
    public void Wait(float howLong)
    {
        Speed = 0.0f;
        waitTime = howLong;
        //
        ChangeState(States.Waiting);
    }

    /// <summary>
    /// Instantly deals damage to a targetted structure and puts the creep into "Attack" state.
    /// </summary>
    /// <param name="s"></param>
    public void Attack(Structure s)
    {
        if (!s.indestructible)
        {
            s.TakeDamage(power);
            //
            s.OnAttacked(this);


            ChangeState(States.Attacking);
        }
    }


    protected override void Awake()
    {
        base.Awake();
        //
        cc.isTrigger = false;
    }

    private void Start()
    {
        destinationPos = Vector2.zero;
        cc.radius = 1.0f;
        turnRate = 45.0f;
        //
        Direction = Helper.PointDirection(transform.position, destinationPos); // ??? <-- More like "Roomgrid center"
        Wait(1.0f);

        Director.Instance().creepCount++;
    }


    public override void Act()
    {
        base.Act();
        //
        if (state == States.Waiting)
        {
            waitTime -= 1 * Time.deltaTime;
            if (waitTime <= 0) { ChangeState(States.Moving); }
        }
        else
        {
            // *** Only project heading if we are NOT busy!
            if (!awareOfCore && !curious)
            {
                if (Helper.PointDistance(pos, destinationPos) < cc.radius)
                {
                    destinationPos = pos + rb.velocity * 1000; // *** Go indefinitely into that direction!
                }
            }
            //
            Direction = Mathf.MoveTowardsAngle(Direction, Helper.PointDirection(transform.position, destinationPos), turnRate / Helper.SECOND);


            // *** If you leave the area, you're dead!
            Roomgrid rg = Roomgrid.Instance();
            if (!rg.ActorInBounds(this)) { Die(); }
        }
    }

    /// <summary>
    /// This is the primary function where Creeps make their decisions.
    /// These involve moving, turning towards a core, avoiding walls, etc...
    /// </summary>
    public override void Assess()
    {
        base.Assess();

        
        Director d = Director.Instance();
        //
        awareOfCore = false; // *** Does it see the core?
        curious = false; // *** Is it following another creep?
        //
        //
        if (state == States.Inactive)
        {
            // *** Take a moment to wake up.
            ChangeState(States.Moving);
            return;
        }
        //
        //
        // *** Check LOS...
        List<PowerCore> cores = Helper.LOSAllWithinRadius<PowerCore>(this, sightRadius, Helper.LAYER_OBSTRUCT | Helper.LAYER_STRUCTURE);
        foreach (PowerCore Q in cores)
        {
            if (Q.isDead) { continue; }


            // *** Go Thataway!
            destinationPos = Q.pos;
            awareOfCore = true; // ??? <-- Pashing! Exclamation point!!
            //
            if (Helper.PointDistance(pos, Q.pos) < (Size + Q.Size))
            {
                Attack(Q);
            }
            break;
        }
        //
        if (state == States.Moving)
        {
            switch (creepType)
            {
                case CreepTypes.Scout:
                    break;
                case CreepTypes.Sheep:
                    if (!awareOfCore)
                    {
                        // *** Flocking behavior!
                        int found = 0;
                        Vector2 center = Vector2.one;
                        foreach (Creep Q in Helper.FindAllWithinRadius<Creep>(this, sightRadius))
                        {
                            if (Q == this) { continue; }
                            else if (Q.isDead) { continue; }
                            else if (Helper.LOS(this, Q.pos, sightRadius, Helper.LAYER_OBSTRUCT) != null) { continue; }


                            if (Q.awareOfCore)
                            {
                                found++;
                                //
                                center = Vector2.Lerp(center, Q.pos, 1 / found);
                            }
                        }
                        //
                        if (found > 0)
                        {
                            curious = true;
                            destinationPos = center;
                        }
                    }
                    break;
            }
            //
            RaycastHit2D ray = Physics2D.Raycast(pos, Helper.AngleToVector2(Direction), cc.radius + Speed, Helper.LAYER_OBSTRUCT);
            if (ray.collider != null)
            {
                // *** Are we headed towards a wall??
                RaycastHit2D rayLeft = Physics2D.Raycast(pos, Helper.AngleToVector2(Direction - turnRate), cc.radius + Speed, Helper.LAYER_OBSTRUCT);
                RaycastHit2D rayRight = Physics2D.Raycast(pos, Helper.AngleToVector2(Direction + turnRate), cc.radius + Speed, Helper.LAYER_OBSTRUCT);
                //
                // *** Would we still hit a wall if we turned left...? Would we still hit it if we turned right...?
                bool hitsLeft = rayLeft.collider != null;
                bool hitsRight = rayRight.collider != null;

                if (hitsLeft && !hitsRight)
                {
                    destinationPos = pos + Helper.AngleToVector2(Direction + turnRate) * 1000;
                }
                else if (hitsRight && !hitsLeft)
                {
                    destinationPos = pos + Helper.AngleToVector2(Direction - turnRate) * 1000;
                }
                else if (hitsRight && hitsLeft)
                {
                    Wait(1.0f);
                    destinationPos = pos - rb.velocity * 1000;
                }
            }
        }
        //
        if (waitTime > 0) { Highlight(Color.gray); }
        else if (awareOfCore) { Highlight(Color.red); }
        else if (curious) { Highlight(Color.blue); }
        else { Highlight(Color.white, false); }
    }

    public override void Highlight(Color col, bool doFlash = true)
    {
        base.Highlight(col, doFlash);
        //
        sprite.sr.color = col;
    }

    public override void Despawn()
    {
        base.Despawn();
    }
    public override void Discard()
    {
        base.Discard();
    }


    /* *** Rules of creep behavior:
     * Upon appearing, it moves towards the center of the map.
     * If there is a collision in front of it, it will steer away as much as possible.
     * If it ever runs into a collision, it will stop momentarily, then turn 180 degrees.
     * If it detects the core (based on its own sight radius), all previous rules are void and it will steer towards the core.
     * */
    public enum CreepTypes
    {
        Scout,/// *** It has infinite sight radius. (green)
        Sheep,/// *** Small sight radius. However, it will steer towards the average area of all the nearby creeps that have located the core. (red)
        Clever,/// *** It will follow creeps that have located the core. Otherwise, it will avoid those creeps. (blue)
        Blind, /// *** No sight whatsoever. (purple)
    }
    public CreepTypes creepType;
    /// <summary>
    /// Change to another type of creep.
    /// A function I was thinking about ahead of its usefulness.
    /// </summary>
    /// <param name="type"></param>
    internal void ChangeType(CreepTypes type)
    {
        creepType = type;
        //
        sightRadius = 1.0f;
        //
        switch (creepType)
        {
            case CreepTypes.Scout:
                sightRadius = 100.0f;
                break;
        }
        //
        AssignSprite("Creep"); // !!! <-- Only one type for now! // "Creep_" + type
        // AssignAtlasSprite("Creep");
    }


    /// <summary>
    /// The "reaction" script to a damage area contacting a creep.
    /// </summary>
    /// <param name="db"></param>
    public void GetShot(DamageArea db)
    {
        db.OnHit(this);
        //
        //
        TakeDamage(db.isSustained ? db.power / Helper.SECOND : db.power);
        //
        db.BreakUp(1);
    }


    public override void Die()
    {
        Despawn();
        //
        if (!isDead)
        {
            Director.Instance().creepCount--;
        }
        //
        isDead = true;
    }

    public override void OnSpawn(Actor by = null)
    {
        base.OnSpawn(by);
        //
        ChangeState(States.Inactive);
    }
}
