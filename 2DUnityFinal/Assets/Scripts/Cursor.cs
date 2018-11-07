using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A game object that follows where you tap in-game.
/// Gives a "tangible" form to the player's cursor.
/// </summary>
public class Cursor : MonoBehaviour {

    public Light lite;

	// Use this for initialization
	void Awake () {
        lite = gameObject.AddComponent<Light>();
        lite.intensity = 5;
        lite.range = 1;
        lite.type = LightType.Point;
        lite.enabled = true;


        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 4.5f);
    }
	
	// Update is called once per frame
	void Update () {
        Joystick j = Joystick.Instance();
        transform.position = new Vector3(j.MousePos.x, j.MousePos.y, transform.position.z);
    }
}
