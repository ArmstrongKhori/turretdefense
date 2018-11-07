using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A "Sound Effect" clip used by the Sounder.
/// </summary>
public class SFX : MonoBehaviour {


    public AudioSource aus;

	// Use this for initialization
	void Start () {
        aus = gameObject.GetComponent<AudioSource>();
        if (aus == null) { aus = gameObject.AddComponent<AudioSource>(); }
    }


    /// <summary>
    /// Loads a clip and plays it.
    /// </summary>
    /// <param name="name"></param>
    public void LoadAndPlay(string name)
    {
        aus.clip = (AudioClip)Resources.Load(name);
        //
        aus.Play(0);
    }
}
