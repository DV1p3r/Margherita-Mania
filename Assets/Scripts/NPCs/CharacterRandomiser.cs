using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRandomiser : MonoBehaviour {    

    // -- General Configuration
    [Header("General Settings")]
    public GameObject CharacterModel = null;
    public string Material_Skin = null;
    public Color[] SkinTones = null;
    public string Material_Clothes = null;
    public Color[] ClothingColours = null;

    // -- Face Configuration
    [Header("Face Settings")]
    public string Material_Eyes = null;
    public Texture[] Eyes = null;
    public string Material_Nose = null;
    public Texture[] Noses = null;
    public string Material_Mouth = null;
    public Texture[] Mouths = null;    

    // -- Body Configuration
    [Header("Body Settings")]
    public int BlendShape_Height = -1;    
    [Range(0,100)]
    public float MinHeight = 50f;
    [Range(0,100)]
    public float MaxHeight = 100f;

    public int BlendShape_Weight = -1;
    [Range(0, 100)]
    public float MinWeight = 30f;
    [Range(0, 100)]
    public float MaxWeight = 70f;

    public int BlendShape_Muscle = -1;
    [Range(0, 100)]
    public float MinMuscle = 30f;
    [Range(0, 100)]
    public float MaxMuscle = 70f;   

    // -- Private Variables
    private bool B_Init = false;
    private List<MaterialSlot> CharacterMaterials = new List<MaterialSlot>();

    private SkinnedMeshRenderer CharacterRenderer = null;

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (!InitChecks()) return;

        foreach (Material CurrentMat in CharacterModel.GetComponent<Renderer>().materials)
        {
            MaterialSlot ms_Temp = new MaterialSlot();
            string s_NewName = CurrentMat.name.Remove(CurrentMat.name.LastIndexOf("(Instance)")-1);
            ms_Temp.Name = s_NewName;
            ms_Temp.Material = CurrentMat;
            CharacterMaterials.Add(ms_Temp);            
        }

        CharacterRenderer = CharacterModel.GetComponent<SkinnedMeshRenderer>();
        B_Init = true;
        StartCoroutine(GenerateBody());
        StartCoroutine(GenerateFace());
    }

    private bool InitChecks()
    {
        // General Configuration Check.
        if (CharacterModel == null || SkinTones == null || Material_Skin == null || ClothingColours == null || Material_Clothes == null)
        {
            Debug.LogError("Invalid General Settings setup on CharacterRandomiser");
            return false; 
        }

        // Material Configuration Check.
        if (Material_Eyes == null || Material_Nose == null || Material_Mouth == null)
        {
            Debug.LogError("Invalid Face Material Name setup on CharacterRandomiser");
            return false;
        }
        if (Eyes == null || Eyes.Length <= 0)
        {
            Debug.LogError("Invalid Eyes Material setup on CharacterRandomiser");
            return false;
        }
        if (Noses == null || Noses.Length <= 0)
        {
            Debug.LogError("Invalid Nose Material setup on CharacterRandomiser");
            return false;
        }
        if (Mouths == null || Mouths.Length <= 0)
        {
            Debug.LogError("Invalid Mouth Material setup on CharacterRandomiser");
            return false;
        }

        // BlendShape Configuration Check.
        if (BlendShape_Height == -1 || BlendShape_Weight == -1 || BlendShape_Muscle == -1)
        {
            Debug.LogError("Invalid Blend Shape Name setup on CharacterRandomiser");
            return false;
        }

        return true;
    }

    private IEnumerator GenerateFace()
    {
        if (B_Init)
        {
            CharacterMaterials[GetMaterialPos(Material_Eyes)].Material.SetTexture("_MainTex", Eyes[Random.Range(0, Eyes.Length*100)/100]);   
            CharacterMaterials[GetMaterialPos(Material_Nose)].Material.SetTexture("_MainTex", Noses[Random.Range(0, Noses.Length * 100) / 100]);
            CharacterMaterials[GetMaterialPos(Material_Mouth)].Material.SetTexture("_MainTex", Mouths[Random.Range(0, Mouths.Length * 100) / 100]);
        }
        yield return null;
    }

    private IEnumerator GenerateBody()
    {
        CharacterRenderer.SetBlendShapeWeight(BlendShape_Height,Random.Range(MinHeight,MaxHeight));
        CharacterRenderer.SetBlendShapeWeight(BlendShape_Muscle, Random.Range(MinMuscle, MaxMuscle));
        CharacterRenderer.SetBlendShapeWeight(BlendShape_Weight, Random.Range(MinWeight, MaxWeight));        
        CharacterRenderer.materials[GetMaterialPos(Material_Skin)].color = SkinTones[Random.Range(0, SkinTones.Length)];
        CharacterRenderer.materials[GetMaterialPos(Material_Clothes)].color = ClothingColours[Random.Range(0, ClothingColours.Length)];

        yield return null;
    }

    private int GetMaterialPos (string s_Material)
    {
        int i_Pos = -1;
        while (i_Pos < CharacterMaterials.Count - 1)
        {
            Debug.Log("i_Pos = " + i_Pos + " | MaterialName = " + CharacterMaterials[i_Pos + 1].Name);
            if (CharacterMaterials[i_Pos + 1].Name == s_Material) { Debug.Log("Found Material: " + s_Material + " | i_Pos = " + i_Pos); return i_Pos + 1; }
            i_Pos++;
        }
        
        return i_Pos;
    }

    private struct MaterialSlot
    {
        public string Name;
        public Material Material;
    }
}