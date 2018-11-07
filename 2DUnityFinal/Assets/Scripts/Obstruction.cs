using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Solid boundaries that actors are affected by.
/// </summary>
public class Obstruction : BaseActor
{

    public BoxCollider2D bc;
    public bool mountable = true; // *** It can be occupied by structures (other than the core).
    // ??? <-- ... This is not working yet.
    

    // Use this for initialization
    protected override void Awake () {
        // base.Awake(); // *** DON'T do this!
        //
        bc.isTrigger = false;
        //
        // AssignSprite("Obstruction"); // Blank	
        //
        bc = gameObject.GetComponent<BoxCollider2D>();
        if (bc == null) { bc = gameObject.AddComponent<BoxCollider2D>(); }
        bc.offset = Vector2.zero;
        bc.size = new Vector2(2.5f, 2.5f);
        // bc.isTrigger = true;

        gameObject.layer = 11;
    }
}
