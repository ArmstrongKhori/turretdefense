using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Singleton class that controls all aspects of sound and music.
/// </summary>
public class Sounder : SystemObj
{

    public const int BUFFERCOUNT = 5;
    //
    public int audioIndex = 0;
    public SFX[] audioBuffer = new SFX[BUFFERCOUNT];

    public bool loopFlip = false;
    public float loopBar;
    public float bpm;
    public float bars;
    public int loopArea;
    public AudioClip music;
    public AudioSource musicA;


    /// <summary>
    /// Plays a sound clip; Nothing fancy.
    /// </summary>
    /// <param name="name">The name of the sound clip (no extension)</param>
    public void PlaySound(string name)
    {
        SFX sfx = audioBuffer[audioIndex];
        sfx.LoadAndPlay(name);
        //
        audioIndex = (audioIndex + 1) % 5;
    }
    /// <summary>
    /// Plays a music file.
    /// WARNING: At the moment, it only supports the two songs that come with the program.
    /// This code is INCOMPLETE!!
    /// </summary>
    /// <param name="name">Name of the song (no extension, after "song_")</param>
    public void PlayMusic(string name)
    {
        // ??? <-- Hard-coded for now.
        music = (AudioClip)Resources.Load("song_" + name);
        //
        if (music != null)
        {
            if (name == "emphasis")
            {
                bpm = 140;
                bars = 17;
                loopBar = 9;
                loopArea = 8;
            }
            else
            {
                bpm = 140;
                bars = 3;
                loopBar = 3;
                loopArea = 1;
            }
            //
            musicA.clip = music;
            musicA.volume = 0.5f;
            musicA.time = 0;
            //
            musicA.Play(0);
        }
    }

    private void Update()
    {
        if (musicA == null) { return; }
        if (music == null) { return; }

        // *** This is responsible for making looping music seamless!
        if (loopFlip)
        {

        }
        else
        {
            if (musicA.time >= (music.length * (bars / (bars + loopArea))))
            {
                musicA.time -= music.length * loopArea / (bars + loopArea);
            }
        }
    }


    protected override void Initialize()
    {
        base.Initialize();
        //
        musicA = gameObject.GetComponent<AudioSource>();
        if (musicA == null) { musicA = gameObject.AddComponent<AudioSource>(); }
        //
        GameObject go;
        for (int i = 0; i < audioBuffer.Length; i++)
        {
            go = new GameObject("SFX_Buffer_" + i);
            go.transform.parent = this.transform;
            SFX sfx = go.AddComponent<SFX>();
            audioBuffer[i] = sfx;
        }
    }



    #region Simple stuff.
    public static Sounder instance;
    public static Sounder Instance() { return instance; }

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
