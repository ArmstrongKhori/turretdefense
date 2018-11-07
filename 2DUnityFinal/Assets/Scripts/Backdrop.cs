using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object intended for visual purposes only.
/// </summary>
public class Backdrop : BaseActor
{
    protected override void Awake()
    {
        base.Awake();
        //
        //
        GameManager gm = GameManager.Instance();
        gm.RegisterBackdrop(this);
    }

    public override void Discard()
    {
        GameManager gm = GameManager.Instance();
        gm.UnregisterBackdrop(this);
        //
        base.Discard();
    }
}
