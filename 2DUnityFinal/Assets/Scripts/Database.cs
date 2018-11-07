using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.IO;
using System;

/// <summary>
/// A singleton that carries all the game data and information (such as names, numbers, and whatnot).
/// </summary>
public class Database : SystemObj
{
    /// <summary>
    /// A "Scenario" has the list of every spawn that will occur for some instance (IE: Level 1, Death March, etc...)
    /// It already has the "Runners", so it can be reused constantly, as long as it is "Reset".
    /// </summary>
    public class SpawnScenario
    {
        public string name = "scenario";
        public List<SpawnSequenceRunner> spawns;
        public List<float> whens;
        public SpawnScenario()
        {
            spawns = new List<SpawnSequenceRunner>();
            whens = new List<float>();
        }

        /// <summary>
        /// Add a spawning sequence and what time it triggers (in relation to when the "scenario" was triggered).
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="when"></param>
        public void Add(SpawnSequenceRunner runner, float when)
        {
            spawns.Add(runner);
            whens.Add(when);
        }

        private float lastTime = -1;
        /// <summary>
        /// Checks the time for if a spawner should be activated.
        /// Has its own "last time" value.
        /// </summary>
        /// <param name="time"></param>
        public void Check(float time)
        {
            for (int i = 0; i < whens.Count; i++)
            {
                if (Helper.Overlap(lastTime, whens[i], time))
                {
                    spawns[i].Activate();
                }
                //
                spawns[i].Run();
            }
            //
            lastTime = time;
        }

        /// <summary>
        /// Reinitializes all Sequence Runners so that they can be used again (IE: Restarting the level)
        /// </summary>
        public void Reset()
        {
            lastTime = -1;
            foreach (SpawnSequenceRunner Q in spawns) { Q.Reset(); }
        }
    }
    /// <summary>
    /// This is the class that "runs" sequences/data.
    /// This class reads and interprets Spawn Sequences.
    /// </summary>
    public class SpawnSequenceRunner
    {
        public Vector2 location;

        public bool active = false;
        public bool terminated = false;
        public float runTime;

        public readonly SpawnSequence sequence;
        public SpawnSequenceRunner(SpawnSequence ss, Vector2 spot)
        {
            sequence = ss;
            location = spot;
        }


        public void Activate()
        {
            active = true;
            terminated = false;
            //
            runTime = 0;
            lastTime = -1;
        }
        private float lastTime = -1;
        public void Run()
        {
            if (active && !terminated)
            {
                runTime += Time.deltaTime;
                //
                foreach (SpawnSequenceData Q in sequence.spawns)
                {
                    if (Helper.Overlap(lastTime, Q.when, runTime))
                    {
                        Director d = Director.Instance();
                        Creep c = d.CreateCreep(Q.type, location + (new Vector2(sequence.deviation, sequence.deviation))); // ??? <-- Randomize the deviation.
                        //
                        c.OnSpawn();
                    }
                }
                //
                lastTime = runTime;
                //
                if (runTime >= sequence.length)
                {
                    Terminate();
                }
            }
        }
        public void Terminate()
        {
            active = false;
            terminated = true;
        }
        public void Reset()
        {
            active = false;
            terminated = false;
        }
    }
    /// <summary>
    /// A "sequence" has a bunch of "datas".
    /// These are used for describing the individual "spawning groups" of a scenario
    /// </summary>
    public class SpawnSequence
    {
        public string name = "sequence";
        public float deviation = 0.0f;
        public float length = 0.0f;

        public List<SpawnSequenceData> spawns = new List<SpawnSequenceData>();
        public void Add(SpawnSequenceData data) {
            spawns.Add(data);
            //
            length = Mathf.Max(data.when, length);
        }
    }
    /// <summary>
    /// "data" is the data about what exactly is being spawned and when
    /// </summary>
    public class SpawnSequenceData
    {
        public Creep.CreepTypes type;
        public float when;
        public SpawnSequenceData(Creep.CreepTypes t, float w)
        {
            type = t;
            when = w;
        }
    }

    /// <summary>
    /// A "LevelInfo" holds all the data for... Well, a level.
    /// This includes the "Scenario" it runs, the Obstruction layout it uses, and the turrets you will have available upon starting.
    /// </summary>
    public class LevelInfo
    {
        public string name = "Level 1";
        public string layout;
        public SpawnScenario scenario;
        public List<Structure.StructureTypes> turrets;

        public LevelInfo() { }
        public LevelInfo(string lay, SpawnScenario stage, List<Structure.StructureTypes> inven)
        {
            layout = lay;
            scenario = stage;
            turrets = inven;
        }
    }



    private Dictionary<string, WeaponData> everyWeaponData;
    /// <summary>
    /// Find the data about a weapon Structures use
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public WeaponData QueryWeaponData(string key) { return everyWeaponData[key]; }
    private Dictionary<string, SpawnSequence> everySpawnSequence;
    /// <summary>
    /// Find the data about a spawning sequence
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public SpawnSequence QuerySpawnSequence(string key) { return everySpawnSequence[key]; }
    private Dictionary<string, SpawnScenario> everySpawnScenario;
    /// <summary>
    /// Find the data about a spawning scenario
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public SpawnScenario QuerySpawnScenario(string key) { return everySpawnScenario[key]; }

    private Dictionary<string, LevelInfo> everyLevelInfo;
    /// <summary>
    /// Find the data about an entire level
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public LevelInfo QueryLevelInfo(string key) { return everyLevelInfo[key]; }



    protected override void Initialize()
    {
        base.Initialize();
        //
        // =====================================================================================================================================
        everyWeaponData = new Dictionary<string, WeaponData>();
        everyWeaponData.Add("PelletGun", new PelletGun());


        // =====================================================================================================================================
        Load_SpawnSequences();

        // =====================================================================================================================================
        Load_SpawnScenarios();

        // =====================================================================================================================================
        Load_LevelInfos();
    }


    /// <summary>
    /// Reads an XML that contains spawn sequences and places them into the database.
    /// </summary>
    private void Load_SpawnSequences()
    {
        string textData = ((TextAsset)Resources.Load("SpawnSequences")).text;
        //
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(textData));
        string xmlPathPattern = "//SpawnSequences/sequence";
        XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);
        //
        everySpawnSequence = new Dictionary<string, SpawnSequence>();
        //
        SpawnSequence ss;
        foreach (XmlNode node in nodeList)
        {
            ss = new SpawnSequence();
            //
            ss.name = node.FirstChild.InnerXml;
            foreach (XmlNode entry in node.SelectNodes("entry"))
            {
                XmlNode o = entry.FirstChild;
                // Debug.Log("Node is: " + o.Name + " (" + o.InnerXml + ")");
                Creep.CreepTypes creepType = (Creep.CreepTypes)Enum.Parse(typeof(Creep.CreepTypes), o.InnerXml);
                o = o.NextSibling;
                float timing = float.Parse(o.InnerXml);
                //
                ss.Add(new SpawnSequenceData(creepType, timing));
            }
            //
            everySpawnSequence.Add(ss.name, ss);
        }

        foreach (SpawnSequence Q in everySpawnSequence.Values) { Debug.Log("Entry: " + Q.name); }
    }
    //
    /// <summary>
    /// Reads an XML that contains the spawning scenarios and places them into the database.
    /// </summary>
    private void Load_SpawnScenarios()
    {
        string textData = ((TextAsset)Resources.Load("SpawnScenarios")).text;
        //
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(textData));
        string xmlPathPattern = "//SpawnScenarios/scenario";
        XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);
        //
        everySpawnScenario = new Dictionary<string, SpawnScenario>();
        //
        SpawnScenario ssN;
        foreach (XmlNode node in nodeList)
        {
            ssN = new SpawnScenario();
            //
            ssN.name = node.FirstChild.InnerXml;
            foreach (XmlNode entry in node.SelectNodes("entry"))
            {
                XmlNode o = entry.FirstChild;
                // Debug.Log("Node is: " + o.Name + " (" + o.InnerXml + ")");
                SpawnSequence sequence = QuerySpawnSequence(o.InnerXml);
                o = o.NextSibling;
                float xx = float.Parse(o.InnerXml);
                o = o.NextSibling;
                float yy = float.Parse(o.InnerXml);
                o = o.NextSibling;
                float timing = float.Parse(o.InnerXml);
                //
                Debug.Log(timing + " timing");
                ssN.Add(new SpawnSequenceRunner(sequence, new Vector2(xx, yy)), timing);
            }
            //
            everySpawnScenario.Add(ssN.name, ssN);
        }

        foreach (SpawnScenario Q in everySpawnScenario.Values) { Debug.Log("Entry: " + Q.name); }
    }
    //
    /// <summary>
    /// Reads an XML that contains level information and places them into the database.
    /// </summary>
    private void Load_LevelInfos()
    {
        string textData = ((TextAsset)Resources.Load("LevelInfos")).text;
        //
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(textData));
        string xmlPathPattern = "//LevelInfos/info";
        XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);
        //
        everyLevelInfo = new Dictionary<string, LevelInfo>();
        //
        LevelInfo ssN;
        foreach (XmlNode node in nodeList)
        {
            ssN = new LevelInfo();
            //
            XmlNode o = node.FirstChild;
            ssN.name = node.FirstChild.InnerXml;
            //
            o = o.NextSibling;
            string layout = o.InnerXml;
            o = o.NextSibling;
            string scenario = o.InnerXml;
            //
            List<Structure.StructureTypes> list = new List<Structure.StructureTypes>();
            foreach (XmlNode entry in node.SelectNodes("turrets/entry"))
            {
                list.Add((Structure.StructureTypes)Enum.Parse(typeof(Structure.StructureTypes), entry.InnerXml));
            }
            //
            ssN.layout = layout;
            ssN.scenario = QuerySpawnScenario(scenario);
            ssN.turrets = list;
            //
            everyLevelInfo.Add(ssN.name, ssN);
        }

        foreach (LevelInfo Q in everyLevelInfo.Values) { Debug.Log("Entry: " + Q.name); }
    }


    #region Simple stuff.
    public static Database instance;
    public static Database Instance() { return instance; }

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
