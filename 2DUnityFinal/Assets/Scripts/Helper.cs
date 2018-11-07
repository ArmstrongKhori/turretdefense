using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just a helper function.
/// Has all my ridiculously-named functions that do mathematical things.
/// </summary>
public static class Helper {

    // *** All the Layer information, already bit-shifted.
    public const int LAYER_BACKDROP = 1 << 8;
    public const int LAYER_STRUCTURE = 1 << 9;
    public const int LAYER_CREEP = 1 << 10;
    public const int LAYER_OBSTRUCT = 1 << 11;
    public const int LAYER_DAMAGEAREA = 1 << 12;
    // ??? <-- This is too hard-coded. I don't like it...

    // *** "One second", in relation to the frame rate.
    public static float SECOND = 1 / Time.fixedDeltaTime; // ??? <-- Check this...
    // *** A "nowhere" spot. Mainly for "removing" an actor without explicitly "destroying" it.
    public static Vector3 DEADZONE = new Vector3(54983, 54934, 93393);

    /// <summary>
    /// Returns a value from 0.0 to 1.0 (related by "time" to "period".)
    /// Useful for timing (such as screen-fading or tweening).
    /// </summary>
    /// <param name="time">The time value</param>
    /// <param name="delay">A delay to time. Useful for multiple Longevity calls.</param>
    /// <param name="period">The period to use. When time reaches this, the value becomes 1.0.</param>
    /// <returns></returns>
	public static float Longevity(float time, float delay, float period)
    {
        return Mathf.Min(Mathf.Max(0, time - delay) / period, 1.0f);
    }
    /// <summary>
    /// Returns true when the "now" time is passed over (as determined by the "prev" and "after" times).
    /// </summary>
    /// <param name="prev">The "previous" time</param>
    /// <param name="now">The time to overlap</param>
    /// <param name="after">The "current" time</param>
    /// <returns></returns>
    public static bool Overlap(float prev, float now, float after)
    {
        return prev < now && now <= after;
    }

    /// <summary>
    /// Returns the absolute distance between two vectors.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float PointDistance(Vector2 a, Vector2 b)
    {
        return (b - a).magnitude;
    }

    /// <summary>
    /// Returns the 360-degree angle of two vectors.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float PointDirection(Vector2 a, Vector2 b)
    {
        return PointDirection(a.x, a.y, b.x, b.y);
    }
    /// <summary>
    /// Returns the 360-degree angle of a set of x and y coordinates
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static float PointDirection(float x1, float y1, float x2, float y2)
    {
        return Mathf.Atan2(y2 - y1, x2 - x1) / (Mathf.PI * 2) * 360;
    }
    /// <summary>
    /// Converts a 360-degree angle to a vector without magnitude
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector2 AngleToVector2(float angle)
    {
        float rad = angle * Mathf.PI * 2 / 360;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }


    /// <summary>
    /// Returns a list of all actors "T" within a "radius" of the actor "origin".
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static List<T> FindAllWithinRadius<T>(Actor origin, float radius) where T : Actor
    {
        List<T> blah = new List<T>();
        //
        // ??? <-- Optimize please...
        foreach (T Q in Object.FindObjectsOfType<T>())
        {
            if (PointDistance(origin.pos, Q.pos) < radius)
            {
                blah.Add(Q);
            }
        }
        //
        return blah;
    }

    /// <summary>
    /// Returns a list of all actors "T" within "radius" of actor "origin".
    /// Also checks "Line of Sight" to make sure there is nothing on layer "masking" blocking it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <param name="masking"></param>
    /// <returns></returns>
    public static List<T> LOSAllWithinRadius<T>(Actor origin, float radius, int masking = 0) where T : Actor
    {
        List<T> blah = new List<T>();
        //
        // ??? <-- Optimize please...
        foreach (T Q in Object.FindObjectsOfType<T>())
        {
            Collider2D col = LOS(origin, Q.pos, radius, masking);
            if (col != null && col.gameObject == Q.gameObject)
            {
                blah.Add(Q);
            }
        }
        //
        return blah;
    }


    /// <summary>
    /// Checks if actor "origin" has line of sight of position "target".
    /// Optionally takes a distance of "dist" and uses layer "masking" for obstruction.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="dist"></param>
    /// <param name="masking"></param>
    /// <returns></returns>
    public static Collider2D LOS(Actor origin, Vector2 target, float dist = 100000f, int masking = 0)
    {
        // XXX <-- I hate that I have to do this...
        bool prevState = origin.cc.enabled;
        origin.cc.enabled = false;
        //
        RaycastHit2D ray = Physics2D.Raycast(origin.pos, target - origin.pos, dist, masking);
        //
        origin.cc.enabled = prevState;
        //
        return ray.collider;
    }

}
