using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This provides some actor-like functionalities without particularly being an actor, per se.
/// That is, it allows you to have a sprite and a position in the world, but DOES NOT queue
/// you up in the game manager's processing list.
/// Used mainly for Backdrops.
/// </summary>
public class BaseActor : MonoBehaviour {
    
    public Vector2 pos { set { transform.position = new Vector3(value.x, value.y, transform.position.z); }  get { return new Vector2(transform.position.x, transform.position.y); } }
    
    // Use this for initialization
    protected virtual void Awake()
    {
        // AssignSprite("NASprite");
    }

    public Sprite sprite;
    /// <summary>
    /// Creates a child object that acts as the object's appearance.
    /// If you already have one, it will change the child object's appearance instead.
    /// </summary>
    /// <param name="name"></param>
    public void AssignSprite(string name)
    {
        if (sprite == null)
        {
            sprite = ((GameObject)Instantiate(Resources.Load("sprite"), this.transform)).GetComponent<Sprite>();
            sprite.Initialize();
        }
        //
        sprite.AssignSprite(name);
    }

    /// <summary>
    /// Assigns a special version of a sprite that uses the Sprite Atlas.
    /// Only really used on the Prepare screen at the moment.
    /// </summary>
    /// <param name="name"></param>
    public void AssignAtlasSprite(string name)
    {
        if (sprite == null)
        {
            sprite = ((GameObject)Instantiate(Resources.Load("atlasspr"), this.transform)).GetComponent<Sprite>();
            sprite.Initialize();
        }
        //
        sprite.AssignSprite(name);
    }



    private bool destroyed = false;
    /// <summary>
    /// Programmatically destroys the object, allowing custom garbage collection and interception
    /// of what happens as it is destroyed (even allowing you to prevent it entirely).
    /// </summary>
    public virtual void Discard()
    {
        if (!destroyed)
        {
            Destroy(this.gameObject);
            //
            destroyed = true;
        }
    }
}
