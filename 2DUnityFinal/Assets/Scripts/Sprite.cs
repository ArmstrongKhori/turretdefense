using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "Visual" component of any actor.
/// This one supports regular old sprites, automating the process of making them appear.
/// </summary>
public class Sprite : MonoBehaviour {

    public bool Visible { set { enabled = value; } get { return enabled; } }

    public SpriteRenderer sr;

    /// <summary>
    /// Sets up a Sprite Renderer and gives it the standard "Sprite" material.
    /// </summary>
    internal virtual void Initialize()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) { sr = gameObject.AddComponent<SpriteRenderer>(); }
        //
        sr.material = (Material)Resources.Load("SpriteMat");
    }

    /// <summary>
    /// Allows designation of a sprite at will.
    /// </summary>
    /// <param name="name">Name of the sprite (no extension)</param>
    internal virtual void AssignSprite(string name)
    {
        Graphics g = Graphics.Instance();
        sr.sprite = g.GetSprite(name);
        //
        if (sr.sprite == null)
        {
            sr.sprite = g.GetSprite("NASprite");
        }
    }
}