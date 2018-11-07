using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special version of a "Sprite" that uses a Sprite Atlas instead of sprites.
/// </summary>
public class AtlasSpr : Sprite {


    public MeshRenderer mr;
    public MeshFilter mf;
    public Mesh m;

    internal override void Initialize()
    {
        // *** Don't do the previous stuff.
        //
        mr = gameObject.GetComponent<MeshRenderer>();
        if (mr == null) { mr = gameObject.AddComponent<MeshRenderer>(); }
        mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null) { mf = gameObject.AddComponent<MeshFilter>(); }


        m = GameManager.Instance().quad;
        mf.mesh = m;
        mr.material = (Material)Resources.Load("AtlasMat"); // SpriteMat
    }

    /// <summary>
    /// Blah blah blah-- UV Coordinates.
    /// ONLY supports a single Sprite Atlas at the moment.
    /// </summary>
    /// <param name="name"></param>
    internal override void AssignSprite(string name)
    {
        // *** Don't do the previous stuff here either
        //
        Graphics.TextureData data = Graphics.Instance().GetTexture(name);
        if (data.name == "") { return; }

        //create our new uv coordinates from the texture atlas data
        Vector2[] uvs = new Vector2[4];

        // ??? <-- Hard-coded for now.
        float w = 1024;
        float h = 1024;

        uvs[0].x = data.x/w;
        uvs[0].y = 1 - (data.y + data.height)/h;
        uvs[1].x = (data.x + data.width)/w;
        uvs[1].y = 1 - data.y/h;
        uvs[2].x = (data.x + data.width)/w;
        uvs[2].y = 1 - (data.y + data.height)/h;
        uvs[3].x = data.x/w;
        uvs[3].y = 1 - data.y/h;

        Mesh mesh = mf.mesh;
        mesh.uv = uvs;
    }
}
