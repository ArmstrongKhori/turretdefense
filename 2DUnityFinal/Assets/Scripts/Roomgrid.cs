using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all spacial and room information.
/// ... Not much here, but it is no less useful!
/// </summary>
public class Roomgrid : SystemObj {

    public float Width;
    public float Height;
    public float GroundZ;



    /// <summary>
    /// Returns whether the point is out of bounds.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool PointInBounds(Vector2 point)
    {
        if (point.x < -Width || point.x > Width)
        {
            return false;
        }
        else if (point.y < -Height || point.y > Height)
        {
            return false;
        }


        return true;
    }
    /// <summary>
    /// Returns whether the Actor (accounting for its size) is out of bounds.
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public bool ActorInBounds(Actor a)
    {
        if (a.pos.x + a.Size < -Width || a.pos.x - a.Size > Width)
        {
            return false;
        }
        else if (a.pos.y + a.Size < -Height || a.pos.y - a.Size > Height)
        {
            return false;
        }


        return true;
    }


    protected override void Initialize()
    {
        base.Initialize();
        //
        // *** No matter the game/screen resolution, the game bounds should never go beyond these.
        Width = 6.25f;
        Height = 5.00f;
        GroundZ = 5.0f; // ??? <-- VERY arbitrary...
    }



    #region Simple stuff.
    public static Roomgrid instance;
    public static Roomgrid Instance() { return instance; }

    // Use this for initialization
    void Awake()
    {
        if (instance == null && this != instance) { instance = this; }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        //
        DontDestroyOnLoad(this.gameObject);


        Initialize();
    }
    #endregion
}

