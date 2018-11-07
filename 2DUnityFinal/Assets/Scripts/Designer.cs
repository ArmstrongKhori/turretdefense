using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton that handles menu stuff, such as placement, generation, and updating.
/// </summary>
public class Designer : SystemObj {

    // *** UI Stuff for Preparation menu
    public GameObject optionPanel;
    public GameObject creepPanel;
    public GameObject mapPanel;
    public GameObject turretPanel;
    public Dropdown levelDropDown;



    const int widthHeight = 6;
    //
    public GameObject[] creepIcons = new GameObject[widthHeight * widthHeight];
    public GameObject[] turretIcons = new GameObject[8];


    protected override void Initialize()
    {
        base.Initialize();
        //
        GameManager gm = GameManager.Instance();
        optionPanel = gm.gameSpace.transform.Find("Prepare").Find("Canvas").Find("OptionPanel").gameObject;
        creepPanel = gm.gameSpace.transform.Find("Prepare").Find("Canvas").Find("CreepPanel").gameObject;
        mapPanel = gm.gameSpace.transform.Find("Prepare").Find("Canvas").Find("MapPanel").gameObject;
        turretPanel = gm.gameSpace.transform.Find("Prepare").Find("Canvas").Find("TurretPanel").gameObject;

        levelDropDown = gm.gameSpace.transform.Find("Prepare").Find("Canvas").Find("LevelDropDown").gameObject.GetComponent<Dropdown>();
        levelDropDown.onValueChanged.AddListener(delegate(int action) { UpdateScreen(); });
    }

    /// <summary>
    /// The initial set-up function for the Prepare screen.
    /// </summary>
    /// <param name="info"></param>
    public void SetupScreen(Database.LevelInfo info)
    {
        Graphics g = Graphics.Instance();

        string name = levelDropDown.options[levelDropDown.value].text;
        //
        GameObject go;


        // *** Fill the creep panel
        for (int n = 0; n < creepIcons.Length; n++)
        {
            go = new GameObject("_CreepOptions");
            go.transform.parent = creepPanel.transform;
            //
            Image i = go.AddComponent<Image>();
            i.color = Color.cyan;
            i.sprite = null;
            //
            RectTransform rt = i.rectTransform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(24, 24);
            rt.localScale = Vector3.one;
            //
            rt.anchoredPosition = new Vector2(8 + (8 + rt.sizeDelta.x) * (n % widthHeight), -8 - (8 + rt.sizeDelta.y) * (n / widthHeight));
            //
            //
            creepIcons[n] = go;
        }
        // ??? <-- (if higher than 16): "And more..."

        // *** Fill the turret panel
        for (int count = 0; count < turretIcons.Length; count++)
        {
            go = new GameObject("_TurretOptions");
            go.transform.parent = turretPanel.transform;
            //
            Image i = go.AddComponent<Image>();
            i.color = Color.cyan;
            i.sprite = null;
            //
            RectTransform rt = i.rectTransform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(128, 40);
            rt.localScale = Vector3.one;
            //
            rt.anchoredPosition = new Vector2(16, -16 - 48 * count);


            // *** Turret's image
            GameObject go_i = new GameObject("_TurretIcon");
            go_i.transform.parent = go.transform;
            //
            i = go_i.AddComponent<Image>();
            i.sprite = null;
            //
            rt = i.rectTransform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(40, 40);
            rt.localScale = Vector3.one;
            //
            rt.anchoredPosition = new Vector2(0, 0);

            // *** Turret's count
            GameObject go_c = new GameObject("_TurretCount");
            go_c.transform.parent = go.transform;
            //
            Text txt = go_c.AddComponent<Text>();
            txt.text = "NA";
            txt.font = (Font)Resources.Load("MVBOLI");
            txt.fontSize = 24;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.LowerRight;
            txt.color = Color.blue;
            //
            rt = txt.rectTransform;
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
            rt.sizeDelta = new Vector2(40, 40);
            rt.localScale = Vector3.one;
            //
            rt.anchoredPosition = new Vector2(-8, 0);
            //
            //
            turretIcons[count] = go;
        }
        //
        UpdateScreen();
    }

    /// <summary>
    /// Updates the various UI elements of the Prepare screen
    /// </summary>
    public void UpdateScreen()
    {
        string name = levelDropDown.options[levelDropDown.value].text;
        //
        


        Graphics g = Graphics.Instance();
        GameManager gm = GameManager.Instance();
        Database.LevelInfo info = Database.Instance().QueryLevelInfo(name);


        // *** Set the map image
        BaseActor ba = gm.gameSpace.transform.Find("Prepare").Find("AtlasLayer").Find("atlasMap").gameObject.GetComponent<BaseActor>();
        ba.AssignAtlasSprite("icon_level_" + name);
        //
        // *** This is "fallback" code in case the Sprite Atlas breaks again.
        Image im = mapPanel.transform.Find("MapImage").GetComponent<Image>();
        UnityEngine.Sprite o = g.GetSprite("Layout_" + name);
        im.sprite = o;


        // *** Get all the turrets in the selected level...
        Dictionary<Structure.StructureTypes, int> turretsInLevel = new Dictionary<Structure.StructureTypes, int>();
        foreach (Structure.StructureTypes Q in info.turrets)
        {
            if (!turretsInLevel.ContainsKey(Q)) { turretsInLevel.Add(Q, 0); }
            //
            turretsInLevel[Q]++;
        }

        // *** Get all the creeps in the selected level...
        List<Creep.CreepTypes> creepsInLevel = new List<Creep.CreepTypes>();
        foreach (Database.SpawnSequenceRunner a in info.scenario.spawns)
        {
            foreach (Database.SpawnSequenceData b in a.sequence.spawns)
            {
                creepsInLevel.Add(b.type);
            }
        }
        //
        //
        GameObject go;
        // *** Fill the creep panel
        for (int n = 0; n < creepIcons.Length; n++)
        {
            if (n < creepsInLevel.Count)
            {
                creepIcons[n].SetActive(true);
                //
                Image i = creepIcons[n].GetComponent<Image>();
                i.sprite = g.GetSprite("icon_creep_" + creepsInLevel[n].ToString().ToLower());
            }
            else
            {
                creepIcons[n].SetActive(false);
                continue;
            }
        }
        // ??? <-- (if higher than 16): "And more..."


        // *** Fill the turret panel
        int count = 0;
        foreach (KeyValuePair<Structure.StructureTypes, int> Q in turretsInLevel)
        {
            go = turretIcons[count];
            go.SetActive(true);
            //
            //
            // *** Turret's image
            go.transform.Find("_TurretIcon").gameObject.GetComponent<Image>().sprite = g.GetSprite("icon_turret_" + Q.Key.ToString().ToLower());

            // *** Turret's count
            go.transform.Find("_TurretCount").gameObject.GetComponent<Text>().text = "x" + (Q.Value).ToString();


            count++;
        }
        // *** Disable the rest of the icons
        for (int i = count; i < turretIcons.Length; i++)
        {
            turretIcons[i].SetActive(false);
        }
    }



    #region Simple stuff.
    public static Designer instance;
    public static Designer Instance() { return instance; }

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
