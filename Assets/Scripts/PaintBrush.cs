using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour {
    public bool B_Active = false;
    public float Range = 1;
    public LayerMask Layers;
    public int resolution = 512;
    Texture2D whiteMap;
    public float brushSize;
    public Texture2D brushTexture;
    [SerializeField]Vector2 stored;
    public static Dictionary<Collider, RenderTexture> paintTextures = new Dictionary<Collider, RenderTexture> ();
    void Start () {
        CreateClearTexture (); // clear white texture to draw on
    }

    void Update ()
    {
        if (B_Active)
        {
            Debug.DrawRay (transform.position, transform.right * Range, Color.magenta);
            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.right, out hit, Range, Layers))
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) // delete previous and uncomment for mouse painting
            {
                Collider coll = hit.collider;
                if (coll != null) {
                    if (!paintTextures.ContainsKey (coll)) // if there is already paint on the material, add to that
                    {
                        Debug.Log(hit.transform.name);
                        Renderer rend = hit.transform.GetComponent<Renderer> ();
                        if (rend.material.HasProperty ("_Draw") != false) 
                        {
                            paintTextures.Add (coll, getRTFromTex (rend));
                            rend.material.SetTexture ("_Draw", paintTextures[coll]);
                        }
                    }
                    if (stored != hit.textureCoord) // stop drawing on the same point
                    {
                        stored = hit.textureCoord;
                        Vector2 pixelUV = hit.textureCoord;
                        pixelUV.y *= resolution;
                        pixelUV.x *= resolution;
                        DrawTexture (paintTextures[coll], pixelUV.x, pixelUV.y);
                    }
                }
            }
        }
    }

    void DrawTexture (RenderTexture rt, float posX, float posY) {

        RenderTexture.active = rt; // activate rendertexture for drawtexture;
        GL.PushMatrix (); // save matrixes
        GL.LoadPixelMatrix (0, resolution, resolution, 0); // setup matrix for correct size

        // draw brushtexture
        Graphics.DrawTexture (new Rect (posX - brushTexture.width / brushSize, (rt.height - posY) - brushTexture.height / brushSize, brushTexture.width / (brushSize * 0.5f), brushTexture.height / (brushSize * 0.5f)), brushTexture);
        GL.PopMatrix ();
        RenderTexture.active = null; // turn off rendertexture
    }
    RenderTexture getRTFromTex (Renderer rend) {
        //RenderTexture rt = new RenderTexture(rend.material.GetTexture("_Draw").width, rend.material.GetTexture("_Draw").height, 32);
        RenderTexture rt = new RenderTexture(resolution, resolution, 32);
        Graphics.Blit (rend.material.GetTexture ("_Draw"), rt);
        return rt;
    }
    RenderTexture getWhiteRT () {
        RenderTexture rt = new RenderTexture (resolution, resolution, 32);
        Graphics.Blit (whiteMap, rt);
        return rt;
    }

    void CreateClearTexture () {
        whiteMap = new Texture2D (1, 1);
        whiteMap.SetPixel (0, 0, Color.clear);
        whiteMap.Apply ();
    }

    //void bake(Collider coll)
    //{
    // coll.gameobject.render.material.GetTexture("_Draw") =  colorTex (paintTextures[coll]);
    // remove the colider key from dic
    //}

    //void colorTex(rendertex tex)
    //DARKERN texture code here
}