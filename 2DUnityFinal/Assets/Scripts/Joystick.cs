using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles general input for tapping, typing, or clicking.
/// </summary>
public class Joystick : SystemObj
{
    /// <summary>
    /// Returns the position of the mouse in relation to the game space
    /// </summary>
    public Vector2 MousePos { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); } }

    public bool mouseLeftDown;
    public bool mouseLeftClick;
    private void Update()
    {
        // *** Check if the mouse was CLICKED, not held.
        bool newVal;
        newVal = Input.GetMouseButton(0);
        //
        mouseLeftClick = newVal && !mouseLeftDown;
        //
        mouseLeftDown = newVal;
    }


    #region Simple stuff.
    public static Joystick instance;
    public static Joystick Instance() { return instance; }

    // Use this for initialization
    void Start()
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
