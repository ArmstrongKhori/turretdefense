using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// THE GameManager-- The big guy! The dude that handles everything!
/// ... Well, not really.
/// It "manages" the "game". Not much more to say.
/// </summary>
public class GameManager : SystemObj
{
    /// <summary>
    /// The "Game Space" game object. VERY important for locating certain things (since a lot of my objects are generated programmatically).
    /// </summary>
    public GameObject gameSpace;


    public Mesh quad; // ??? <-- ... Don't ask.


    protected override void Initialize()
    {
        // *** Link all the objects to their respective states
        gameStateMap = new Dictionary<GameStates, GameObject>();
        gameStateMap.Add(GameStates.Intro, gameSpace.transform.Find("Intro").gameObject);
        gameStateMap.Add(GameStates.Menu, gameSpace.transform.Find("Menu").gameObject);
        gameStateMap.Add(GameStates.Prepare, gameSpace.transform.Find("Prepare").gameObject);
        gameStateMap.Add(GameStates.Game, gameSpace.transform.Find("Game").gameObject);
        //
        foreach (GameObject Q in gameStateMap.Values) { Q.SetActive(false); }



        // *** Order matters here!
        GameObject go;
        go = new GameObject("Database");
        go.transform.parent = this.transform;
        go.AddComponent<Database>();
        //
        go = new GameObject("Roomgrid");
        go.transform.parent = this.transform;
        go.AddComponent<Roomgrid>();
        //
        go = new GameObject("Graphics");
        go.transform.parent = this.transform;
        go.AddComponent<Graphics>();
        //
        go = new GameObject("Designer");
        go.transform.parent = this.transform;
        go.AddComponent<Designer>();
        //
        //
        queueAddActor = new List<Actor>();
        queueRemoveActor = new List<Actor>();
        RefreshRegistry();

        //
        //

        
    }

    

    // Update is called once per frame
    void FixedUpdate () {
        RegistryResolve();
        //
        RunState();
        //
		foreach (Actor Q in allActors)
        {
            if (Q.alive)
            {
                Q.Act();
            }
        }
	}


    public string targetLevel = "";

    /// <summary>
    /// A function used by the "Prepare" screen to initate game mode.
    /// </summary>
    public void DoBattleMode()
    {
        if (gameState == GameStates.Prepare)
        {
            Designer de = Designer.Instance();
            targetLevel = de.levelDropDown.options[de.levelDropDown.value].text;
            //
            ChangeModeState(GameStateModes.Ending);
        }
    }



    #region Game state and such
    public enum GameStateModes
    {
        Beginning,
        Running,
        Ending,
    }
    /// <summary>
    /// A sort of "sub state" for the game manager.
    /// Used mainly for the fade ins/outs from the curtain.
    /// </summary>
    public GameStateModes stateMode = GameStateModes.Running;
    /// <summary>
    /// Changes the sub-state of the game.
    /// </summary>
    /// <param name="gsm"></param>
    public void ChangeModeState(GameStateModes gsm) {
        stateMode = gsm;
        modeTime = 0;
    }
    public enum GameStates
    {
        Initialize,
        Intro,
        Menu,
        Prepare,
        Game,
    }
    /// <summary>
    /// Represents the game's current state.
    /// Shifts when going from screen to screen.
    /// </summary>
    public GameStates gameState = GameStates.Initialize;
    /// <summary>
    /// Has the related gameobject for each game state.
    /// </summary>
    public Dictionary<GameStates, GameObject> gameStateMap;
    public int stateTime;
    public int modeTime;

    /// <summary>
    /// "Runs" the game states.
    /// Used for doing stuff that needs to be checked rapidly (particularly, the screen fading).
    /// </summary>
    public void RunState()
    {
        stateTime += 1;
        modeTime += 1;
        //
        float fade;
        Graphics g = Graphics.Instance();
        Joystick j = Joystick.Instance();
        //
        //
        // *** Screen-fading stuff.
        switch (stateMode)
        {
            case GameStateModes.Beginning:
                fade = Helper.Longevity(modeTime, 0, (0.3f) * Helper.SECOND);
                g.curtain.color = new Color(0, 0, 0, 1 - fade);
                break;
            case GameStateModes.Running:
                break;
            case GameStateModes.Ending:
                fade = Helper.Longevity(modeTime, 0, (0.3f) * Helper.SECOND);
                g.curtain.color = new Color(0, 0, 0, fade);
                break;
        }
        //
        // *** This is a doozy! Every state of the game is managed here!
        switch (gameState)
        {
            // ------------------------------------------------------------------------------------------------------------
            case GameStates.Initialize:
                ChangeState(GameStates.Intro);
                break;
            // ------------------------------------------------------------------------------------------------------------
            case GameStates.Intro:
                fade = Helper.Longevity(stateTime, 0, 2 * Helper.SECOND);
                fade -= Helper.Longevity(stateTime, (2 +3) * Helper.SECOND, 2 * Helper.SECOND);
                g.curtain.color = new Color(0, 0, 0, 1-fade);


                if (stateTime > (2 + 3 + 2) * Helper.SECOND)
                {
                    // g.curtain.color = Color.clear;
                    //
                    ChangeState(GameStates.Menu);
                }
                break;
            // ------------------------------------------------------------------------------------------------------------
            case GameStates.Menu:
                switch (stateMode) {
                    case GameStateModes.Beginning:
                        if (modeTime >= Helper.SECOND) { ChangeModeState(GameStateModes.Running); }
                        break;
                    case GameStateModes.Running:
                        // !!! <-- 2D creeps march by, generated at somewhat random intervals.

                        if (j.mouseLeftClick)
                        {
                            ChangeModeState(GameStateModes.Ending);
                        }
                        break;
                    case GameStateModes.Ending:
                        if (modeTime >= Helper.SECOND) { ChangeState(GameStates.Prepare); }
                        break;
                }
                break;
            // ------------------------------------------------------------------------------------------------------------
            case GameStates.Prepare:
                switch (stateMode)
                {
                    case GameStateModes.Beginning:
                        if (modeTime >= Helper.SECOND) { ChangeModeState(GameStateModes.Running); }
                        break;
                    case GameStateModes.Running:
                        break;
                    case GameStateModes.Ending:
                        if (modeTime >= Helper.SECOND) { ChangeState(GameStates.Game); }
                        break;
                }
                break;
            // ------------------------------------------------------------------------------------------------------------
            case GameStates.Game:
                switch (stateMode)
                {
                    case GameStateModes.Beginning:
                        if (modeTime >= Helper.SECOND) { ChangeModeState(GameStateModes.Running); }
                        break;
                    case GameStateModes.Running:
                        Director d = Director.Instance();
                        d.Run(this);
                        break;
                    case GameStateModes.Ending:
                        if (modeTime >= Helper.SECOND) { ChangeState(GameStates.Prepare); }
                        break;
                }
                break;
                // ------------------------------------------------------------------------------------------------------------
        }
    }
    /// <summary>
    /// Changes the game's state.
    /// Essentially transitions the screen from one to another one (intro to menu, etc...)
    /// </summary>
    /// <param name="st"></param>
    public void ChangeState(GameStates st)
    {
        Sounder s = Sounder.Instance();
        switch (gameState)
        {
            case GameStates.Prepare:
                s.musicA.Stop();
                break;
            case GameStates.Game:
                s.musicA.Stop();



                Director d = Director.Instance();
                d.Release();


                // ??? <-- It sorta defeats the purpose of making all of those object pools if I just delete them like this...
                RemoveAllThings();
                break;
        }
        //
        if (gameStateMap.ContainsKey(gameState)) { gameStateMap[gameState].SetActive(false); }
        //
        gameState = st;
        stateTime = 0;
        //
        ChangeModeState(GameStateModes.Beginning);
        //
        if (gameStateMap.ContainsKey(gameState)) { gameStateMap[gameState].SetActive(true); }
        //
        switch (gameState)
        {
            case GameStates.Prepare:
                s.PlayMusic("jazzy");

                Designer de = Designer.Instance();
                de.SetupScreen(Database.Instance().QueryLevelInfo("Level 1")); // ??? <-- BAD.
                break;
            case GameStates.Game:
                s.PlayMusic("emphasis");



                GameObject go;
                go = new GameObject("Director");
                go.transform.parent = this.transform;
                go.AddComponent<Director>();
                break;
        }
    }

    /// <summary>
    /// Destroys all actors currently on the screen.
    /// Used for clean-up when the game state ends.
    /// </summary>
    public void RemoveAllThings()
    {
        List<BaseActor> destroyList = new List<BaseActor>();
        //
        foreach (Actor Q in allActors) { destroyList.Add(Q); }
        foreach (Backdrop Q in allBackdrops) { destroyList.Add(Q); }
        //
        while (destroyList.Count > 0)
        {
            destroyList[0].Discard();
            //
            destroyList.RemoveAt(0);
        }
        //
        allActors.Clear();
        allBackdrops.Clear();
        //
        //
        RefreshRegistry();
    }
    #endregion


    #region Actor and backdrop registration. Makes them show up and such.
    /// <summary>
    /// Refreshes the registry of actors.
    /// Basically, double-checks for if there are any actors/backdrops it might have missed.
    /// </summary>
    private void RefreshRegistry()
    {
        allActors.Clear();
        allBackdrops.Clear();
        //
        foreach (Backdrop Q in FindObjectsOfType<Backdrop>())
        {
            RegisterBackdrop(Q);
        }
        //
        foreach (Actor Q in FindObjectsOfType<Actor>())
        {
            RegisterActor(Q);
        }
    }

    public List<Actor> allActors;
    private List<Actor> queueAddActor;
    private List<Actor> queueRemoveActor;
    /// <summary>
    /// Queues up the actor to be added to the registry.
    /// This is to prevent the enumerator from being modified during the game loop.
    /// </summary>
    /// <param name="a"></param>
    public void RegisterActor(Actor a)
    {
        queueAddActor.Add(a);
    }
    /// <summary>
    /// Queues up the actor to be removed from the registry.
    /// This is to prevent the enumerator from being modified during the game loop.
    /// </summary>
    /// <param name="a"></param>
    public void UnregisterActor(Actor a)
    {
        queueRemoveActor.Add(a);
    }
    /// <summary>
    /// This is what ACTUALLY adds actors to the registry.
    /// </summary>
    /// <param name="a"></param>
    public void _RegisterActor(Actor a)
    {
        if (!allActors.Contains(a))
        {
            allActors.Add(a);
        }
    }
    /// <summary>
    /// This is what ACTUALLY removes actors from the registry.
    /// </summary>
    /// <param name="a"></param>
    public void _UnregisterActor(Actor a)
    {
        int index = allActors.IndexOf(a);
        if (index >= 0)
        {
            allActors.RemoveAt(index);
        }
    }

    public List<Backdrop> allBackdrops;
    /// <summary>
    /// Registers a backdrop.
    /// Does not need to be queued up like an actor (for now).
    /// </summary>
    /// <param name="a"></param>
    public void RegisterBackdrop(Backdrop a)
    {
        if (!allBackdrops.Contains(a))
        {
            allBackdrops.Add(a);
        }
    }
    /// <summary>
    /// Unregisters a backdrop.
    /// Does not need to be queued up like an actor (for now).
    /// </summary>
    /// <param name="a"></param>
    public void UnregisterBackdrop(Backdrop a)
    {
        int index = allBackdrops.IndexOf(a);
        if (index >= 0)
        {
            allBackdrops.RemoveAt(index);
        }
    }

    /// <summary>
    /// Dequeues all the registering and unregistering actors and backdrops and DOES it.
    /// </summary>
    private void RegistryResolve()
    {
        foreach (Actor Q in queueAddActor)
        {
            _RegisterActor(Q);
        }
        //
        foreach (Actor Q in queueRemoveActor)
        {
            _UnregisterActor(Q);
        }
    }
    #endregion


    #region Simple stuff.
    public static GameManager instance;
    public static GameManager Instance() {
        return instance;
    }

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
