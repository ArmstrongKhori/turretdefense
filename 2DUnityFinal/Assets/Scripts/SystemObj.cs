using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An "intermediary" class that helps me recognize what things are essential to game operation.
/// In the future, I would put more here (such as "automatic" singleton code and generic stuff).
/// </summary>
public class SystemObj : MonoBehaviour {

    /// <summary>
    /// Called the moment something is created.
    /// Slower than Awake(), but faster than Start().
    /// </summary>
    protected virtual void Initialize() { }

    
    /// <summary>
    /// Deletes the object programmatically.
    /// </summary>
    public void Discard()
    {
        Destroy(this.gameObject);
    }

}
