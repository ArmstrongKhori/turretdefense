using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton that handles all the things that happen during "game time".
/// Basically, a "mini-gamemanager" for gameplay.
/// </summary>
public class Director : SystemObj
{
    public Database.LevelInfo levelInfo;
    public float battleTime = 0;
    public int creepCount = 0;

    public Slider timeSlider;

    public void RefreshTimeScale()
    {
        Time.timeScale = timeSlider.value;
    }

    /// <summary>
    /// Creates the base game object that all other create events use.
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    private GameObject _CreateObject(Vector2 spot)
    {
        Roomgrid rg = Roomgrid.Instance();


        GameManager gm = GameManager.Instance();
        //
        //
        GameObject go = new GameObject("_gameobject");
        go.transform.parent = gm.gameStateMap[GameManager.GameStates.Game].transform; // *** Place it into the "game" area.
        //
        go.transform.localPosition = new Vector3(rg.Width * spot.x, rg.Height * spot.y, rg.GroundZ - 0.1f);
        go.transform.localScale = Vector3.one * 0.25f; // *** 25% smaller...


        return go;
    }

    public Structure coreObj;
    /// <summary>
    /// Creates a power core, the center of the game.
    /// </summary>
    /// <param name="spot"></param>
    /// <returns></returns>
    public PowerCore CreateCore(Vector2 spot)
    {
        GameObject go = _CreateObject(spot);
        go.name = "Core";
        go.layer = 9;
        //
        PowerCore pc = go.AddComponent<PowerCore>();
        pc.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //
        if (coreObj == null) { coreObj = pc; }


        return pc;
    }

    /// <summary>
    /// Creates a structure, the entities that defend the power core
    /// </summary>
    /// <param name="type"></param>
    /// <param name="spot"></param>
    /// <returns></returns>
    public Structure CreateStructure(Structure.StructureTypes type, Vector2 spot)
    {
        GameObject go = _CreateObject(spot);
        go.name = "Turret_" + type;
        go.layer = 9;
        //
        Structure s = go.AddComponent<Structure>();
        s.ChangeType(type);
        s.rb.constraints = RigidbodyConstraints2D.FreezePosition;


        return s;
    }

    /// <summary>
    /// Creates a creep, the entities that seek out the power core
    /// </summary>
    /// <param name="type"></param>
    /// <param name="spot"></param>
    /// <returns></returns>
    public Creep CreateCreep(Creep.CreepTypes type, Vector2 spot)
    {
        GameObject go = _CreateObject(spot);
        go.name = "Creep_" + type;
        go.layer = 10;
        go.transform.localScale *= 0.5f;
        //
        Creep c = go.AddComponent<Creep>();
        c.ChangeType(type); // *** Automatically sets the sprite!
        c.rb.constraints = RigidbodyConstraints2D.FreezeRotation;


        return c;
    }

    /// <summary>
    /// Creates a Damage Area, the manner in which Structures fight Creeps.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spot"></param>
    /// <returns></returns>
    public T CreateDamageArea<T> (Vector2 spot) where T : DamageArea
    {
        GameObject go = _CreateObject(spot);
        go.name = "DamageArea_" + typeof(T);
        go.layer = 12;
        //
        T c = go.AddComponent<T>();


        return c;
    }

    /// <summary>
    /// Creates a Backdrop, a static, visual object.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="spot"></param>
    /// <returns></returns>
    public Backdrop CreateBackdrop(string name, Vector2 spot)
    {
        Roomgrid rg = Roomgrid.Instance();


        GameObject go = _CreateObject(spot);
        go.name = "_backdrop";
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, rg.GroundZ);
        go.transform.localScale = Vector3.one * 1.0f; // *** Normal size
        go.layer = 8;
        //
        Backdrop b = go.AddComponent<Backdrop>();
        b.AssignSprite(name);
        b.sprite.sr.material = (Material)Resources.Load("GroundMat");
        //


        return b;
    }


    public GameObject layoutSpace;
    public Cursor gameCursor;
    protected override void Initialize()
    {
        GameManager gm = GameManager.Instance();
        timeSlider = gm.gameSpace.transform.Find("Game").Find("Canvas").Find("TimeSlider").GetComponent<Slider>();
        timeSlider.onValueChanged.AddListener(delegate { RefreshTimeScale(); Debug.Log("YO!"); });

        layoutSpace = gm.gameSpace.transform.Find("Game").Find("Levels").gameObject;
        gameCursor = gm.gameSpace.transform.Find("Game").Find("GameCursor").gameObject.GetComponent<Cursor>();


        Backdrop b = CreateBackdrop("Ground", new Vector2(0, 0));
        SpriteRenderer sr = b.sprite.sr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(40, 30);


        ChangePhase(Phases.Initialize);
    }


    /// <summary>
    /// Upon defeat, all structures are destroyed.
    /// </summary>
    public void DoDefeat()
    {
        GameManager gm = GameManager.Instance();
        foreach (Actor Q in gm.allActors)
        {
            Structure s = Q as Structure;
            if (s == null) { continue; }


            if (!s.isDead) { s.Die(); }
        }
        //
        ChangePhase(Phases.Defeat);
    }


    int _placementIndex = 0;
    Structure placeTurret;
    public enum Phases
    {
        Initialize, // *** Default phase
        Begin, // *** Fade in, get introduced.
        PlaceCore, // *** Place the Core
        PlaceTurrets, // *** Place defenses
        PreBattle, // *** Prepare for swarm...
        Battle, // *** (gameplay)
        PostBattle, // *** Check battle stats/info...
        Victory, // *** Did the core survive?
        Defeat, // *** The core blewed up!
        Finish, // *** Fade out
    }
    public Phases phase = Phases.Initialize;
    public int phaseTime = 0;
    public void ChangePhase(Phases p)
    {
        GameManager gm = GameManager.Instance();
        Graphics g = Graphics.Instance();
        g.announcementText.text = "";
        //
        // *** Ending phase
        switch (phase)
        {

        }
        //
        phase = p;
        phaseTime = 0;
        //
        // *** Starting phase
        Transform layout;
        switch (phase)
        {
            case Phases.Initialize:
                levelInfo = Database.Instance().QueryLevelInfo(gm.targetLevel);
                //
                levelInfo.scenario.Reset();
                //
                layout = layoutSpace.transform.Find(levelInfo.name);
                if (layout != null)
                {
                    layout.gameObject.SetActive(true);
                }

                timeSlider.value = 1.0f;
                timeSlider.enabled = false;
                break;
            case Phases.Begin:
                break;
            case Phases.PlaceCore:
                g.announcementText.text = "Place the Core!";
                g.announcementText.color = Color.cyan;

                CreateCore(new Vector2(0, 0));
                break;
            case Phases.PlaceTurrets:
                g.announcementText.text = "Place your turrets!";
                g.announcementText.color = Color.cyan;

                _placementIndex = 0;
                placeTurret = null;
                break;
            case Phases.Battle:
                battleTime = 0;

                timeSlider.enabled = true;
                break;
            case Phases.Victory:
                g.announcementText.text = "Hey!\nYou did it!!";
                g.announcementText.color = Color.yellow;

                timeSlider.value = 1.0f;
                timeSlider.enabled = false;
                break;
            case Phases.Defeat:
                g.announcementText.color = Color.red;
                if (coreObj.isDead)
                {
                    g.announcementText.text = "Oh no!\nThe core is destroyed...";
                }
                else
                {
                    g.announcementText.text = "Game Over!";
                }

                timeSlider.value = 1.0f;
                timeSlider.enabled = false;
                break;
            case Phases.Finish:
                g.announcementText.text = "";
                gm.ChangeModeState(GameManager.GameStateModes.Ending);
                //
                levelInfo = Database.Instance().QueryLevelInfo(gm.targetLevel);
                layout = layoutSpace.transform.Find(levelInfo.name);
                if (layout != null)
                {
                    layout.gameObject.SetActive(false);
                }
                break;
        }
    }


    /// <summary>
    /// Gets called every step, but NOT by the built-in update function!
    /// </summary>
    /// <param name="gm"></param>
    internal void Run(GameManager gm)
    {
        phaseTime += 1;
        //
        Joystick j = Joystick.Instance();
        Roomgrid rg = Roomgrid.Instance();
        switch (phase)
        {
            // *** Allow 1 step for things to initialize and start.
            case Phases.Initialize:
                ChangePhase(Phases.Begin);
                break;
            case Phases.Begin:
                ChangePhase(Phases.PlaceCore);
                break;
            case Phases.PlaceCore:
                coreObj.transform.Translate(gameCursor.transform.position.x - coreObj.transform.position.x, gameCursor.transform.position.y - coreObj.transform.position.y, 0);
                // coreObj.pos = gameCursor.transform.position;
                //
                if (coreObj.colCount > 0)
                {
                    gameCursor.lite.color = Color.red;
                }
                else
                {
                    gameCursor.lite.color = Color.white;

                    if (!rg.PointInBounds(coreObj.pos)) { } // *** Intentionally using "point", not "actor", because I don't want it to hang off the side of the screen...
                    else if (j.mouseLeftClick)
                    {
                        coreObj.OnSpawn();
                        //
                        ChangePhase(Phases.PlaceTurrets);
                    }
                }
                break;
            case Phases.PlaceTurrets:
                if (placeTurret == null)
                {
                    placeTurret = CreateStructure(levelInfo.turrets[_placementIndex], new Vector2(0.0f, 0.2f));
                }
                //
                placeTurret.pos = gameCursor.transform.position;
                //
                if (placeTurret.colCount > 0)
                {
                    gameCursor.lite.color = Color.red;
                }
                else
                {
                    gameCursor.lite.color = Color.white;

                    if (!rg.PointInBounds(placeTurret.pos)) { } // *** Intentionally using "point", not "actor", because I don't want it to hang off the side of the screen...
                    else if (j.mouseLeftClick)
                    {
                        placeTurret.OnSpawn();
                        //
                        _placementIndex++;
                        placeTurret = null;
                        //
                        if (_placementIndex >= levelInfo.turrets.Count)
                        {
                            ChangePhase(Phases.PreBattle);
                        }
                    }
                }
                break;
            case Phases.PreBattle:
                gameCursor.lite.color = Color.white;
                //
                ChangePhase(Phases.Battle);
                break;
            case Phases.Battle:
                // ??? <-- ...?!?!
                battleTime += Time.deltaTime;
                //
                levelInfo.scenario.Check(battleTime);
                //
                bool test = true;
                if (creepCount > 0) { test = false; }
                else
                {
                    foreach (Database.SpawnSequenceRunner Q in levelInfo.scenario.spawns)
                    {
                        if (!Q.terminated)
                        {
                            test = false;
                        }
                    }
                }
                if (test)
                {
                    ChangePhase(Phases.Victory);
                }
                break;
            case Phases.Victory:
                if (phaseTime >= 5*Helper.SECOND)
                {
                    ChangePhase(Phases.PostBattle);
                }
                break;
            case Phases.Defeat:
                if (phaseTime >= 5 * Helper.SECOND)
                {
                    ChangePhase(Phases.PostBattle);
                }
                break;
            case Phases.PostBattle:
                ChangePhase(Phases.Finish);
                break;
            case Phases.Finish:
                break;
        }
    }


    #region Simple stuff.
    public static Director instance;
    public static Director Instance() { return instance; }

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

    public void Release()
    {
        Debug.Log("Destroying " + this.gameObject);
        Destroy(this.gameObject);
        //
        instance = null;
    }
    #endregion
}
