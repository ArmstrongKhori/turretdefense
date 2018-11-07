using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that represents any "thing" that can "exist" in the game, per se.
/// In the context of this game, that is Structures, Creeps, Damage Areas, and Backdrops.
/// Every actor has a physical body, a speed they are moving, and a direction they are facing.
/// </summary>
public class Actor : BaseActor
{

    public bool alive = true;
    public int runTime = 0;

    public Rigidbody2D rb;
    public CircleCollider2D cc;
    private float speed;
    /// <summary>
    /// The speed the actor is currently moving.
    /// This works in tandem with the "Direction" property, allowing you to stop and start moving WITHOUT providing a heading.
    /// </summary>
    public float Speed { set { speed = value; rb.velocity = Helper.AngleToVector2(Direction) * Speed; } get { return speed; } }
    private float direction;
    /// <summary>
    /// The direction the actor is currently facing.
    /// Unlike a Vector, this will RETAIN your directionality, even when your velocity becomes 0.
    /// </summary>
    public float Direction { set { direction = value; rb.velocity = Helper.AngleToVector2(Direction) * Speed; } get { return direction; } }
    /// <summary>
    /// An easy way of setting the Z-depth of an object in 2D space.
    /// </summary>
    public float Depth { set { transform.position = new Vector3(transform.position.x, transform.position.y, value); } get { return transform.position.z; } }
    /// <summary>
    /// An easy way of accessing the "size" of the object in the space.
    /// It accounts for the object's scale, since colliders are not aware of that.
    /// </summary>
    public float Size { get { return cc.radius * transform.localScale.x; } } // ??? <-- Hmpf.


    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        //
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody2D>(); }
        rb.gravityScale = 0.0f;
        rb.drag = 0.0f;
        rb.angularDrag = 0.0f;


        cc = gameObject.GetComponent<CircleCollider2D>();
        if (cc == null) { cc = gameObject.AddComponent<CircleCollider2D>(); }
        cc.radius = 0.25f;
        cc.isTrigger = true;
        //
        //
        GameManager gm = GameManager.Instance();
        gm.RegisterActor(this);
    }



    /// <summary>
    /// This function is called by the Game Manager, which runs this on
    /// every existing Actor whenever its own FixedUpdate() function is called.
    /// This essentially means Actors don't "act" until the Game Manager says so,
    /// easily allowing mechanics such as "pausing", time-control, etc...
    /// </summary>
    public virtual void Act()
    {
        runTime += 1;
        // *** Do stuff here.
    }

    /// <summary>
    /// Signals the destruction/death of this actor.
    /// This is different from being "Discarded" because despawning (in the context of an actor)
    /// does not necessarily mean dying or being removed right away (for example, explosions).
    /// This is especially relevant when Object Pooling.
    /// </summary>
    public virtual void Despawn()
    {
        // *** Do stuff here.
        //
        Discard();
    }
    public override void Discard()
    {
        GameManager gm = GameManager.Instance();
        gm.UnregisterActor(this);
        //
        base.Discard();
    }

    /// <summary>
    /// Called by whatever has caused this actor to exist.
    /// It is not automatic because the creation of an actor
    /// does not necessarily imply the "spawning" of an actor.
    /// </summary>
    /// <param name="by"></param>
    public virtual void OnSpawn(Actor by = null) { }
}