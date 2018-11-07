using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton that handles many graphical aspects of the game, including
/// references to sprites and such.
/// </summary>
public class Graphics : SystemObj
{

    // *** Some random information... Ignore this.
    // 12.51,10
    // 648,518

    public struct TextureData
    {
        public string name;
        public float x;
        public float y;
        public float width;
        public float height;
    }



    /// <summary>
    /// A blank image that functions as a screen-fader.
    /// </summary>
    public Image curtain;
    /// <summary>
    /// The text that appears on the top-left of the screen.
    /// </summary>
    public Text announcementText;


    /// <summary>
    /// Returns a unity sprite. Nothing fancy.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Sprite GetSprite(string name)
    {
        return (UnityEngine.Sprite)Resources.Load(name, typeof(UnityEngine.Sprite));
    }


    /// <summary>
    /// *sigh*
    /// </summary>
    public Mesh quad;


    public Dictionary<string, TextureData> spriteInfos;
    public TextureData GetTexture(string name) { return spriteInfos[name]; }
    protected override void Initialize()
    {
        // ==================================================================================================================================
        Load_SpriteAtlases();

        // ------------------------------------------------------------------------------------------------------------------------------------
        //
        //
        GameManager gm = GameManager.Instance();
        curtain = gm.gameSpace.transform.Find("CurtainCanvas").Find("Curtain").GetComponent<Image>();
        announcementText = gm.gameSpace.transform.Find("CurtainCanvas").Find("Panel").Find("AnnounceText").GetComponent<Text>();
    }

    /// <summary>
    /// Loads all the Sprite Atlas information.
    /// </summary>
    public void Load_SpriteAtlases()
    {
        // ??? <-- At the moment, only the "map preview" uses this.
        string textData = ((TextAsset)Resources.Load("SpriteAtlas")).text;
        //
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(textData));
        string xmlPathPattern = "//SpriteAtlas/entry";
        XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);
        //
        spriteInfos = new Dictionary<string, TextureData>();
        //
        TextureData td;
        foreach (XmlNode node in nodeList)
        {
            td = new TextureData();
            //
            XmlNode o = node.FirstChild;
            td.name = node.FirstChild.InnerXml;
            //
            o = o.NextSibling;
            td.x = int.Parse(o.InnerXml);
            o = o.NextSibling;
            td.y = int.Parse(o.InnerXml);
            o = o.NextSibling;
            td.width = int.Parse(o.InnerXml);
            o = o.NextSibling;
            td.height = int.Parse(o.InnerXml);
            //
            spriteInfos.Add(td.name, td);
        }

        foreach (TextureData Q in spriteInfos.Values) { Debug.Log("Entry: " + Q.name); }
    }



    #region Simple stuff.
    public static Graphics instance;
    public static Graphics Instance() { return instance; }

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
