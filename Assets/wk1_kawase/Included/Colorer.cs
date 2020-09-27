using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data.SqlClient;

[ExecuteInEditMode]
public class Colorer : MonoBehaviour
{
    public Color color;
    [Range(1, 10)]
    public float emissivity = 1.0f;
    //[SyncVar]
    Color currentColor;
    [SerializeField]
    GameObject target;
    private Material m;
    public static Dictionary<Color,Material> colorsinuse;
    private void Awake()
    {
        if (Application.isPlaying)                                      
        {
            if (colorsinuse == null)
                colorsinuse = new Dictionary<Color, Material>();        //only setup colors in use if game is running
            else if (colorsinuse.ContainsKey(color))
                m = colorsinuse[color];                                 //check if material already exists before creating new one
            else
            {
                m = NewMaterial(color, emissivity);
                colorsinuse.Add(color, m);
            }    
        }
        else
            m = NewMaterial(color, emissivity);                          //if in edit mode material sharing is impossible.
            

        if (target == null)
            target = gameObject;
        target.GetComponent<MeshRenderer>().material = m;
        currentColor = color;


    }

    public Material NewMaterial(Color c, float em)
    {
        Material d = new Material(Shader.Find("Universal Render Pipeline/Lit"));// "Standard" for bulitin renderer
        d.SetColor("_BaseColor", c);
        d.SetColor("_EmissionColor", c * em);
        d.EnableKeyword("_EMISSION");
        d.enableInstancing = true;
        return d;
    }


    public void OnEnable()
    {

        if (colorsinuse == null)                                        //shouldn't be necessary but it fixes a bug rn so...
            Awake();

        if (Application.isPlaying )
        {
            ChangeColor(color, emissivity);
        }
        else
        {
            m.SetColor("_BaseColor", color);
            m.SetColor("_EmissionColor", color * emissivity);
        }
    }

    public void OnDisable()
    {
        if(Application.isPlaying)
            ChangeColor(Color.white, 1.0f);
        else
        { 
            m.SetColor("_BaseColor", Color.white);
            m.SetColor("_EmissionColor", Color.white * emissivity);
        }

    }

    public void ChangeColor(Color c, float em)
    {
        if (!Application.isPlaying)                                         //function creates garbage in edit mode so dissallow it
            { Debug.LogWarning("Change Color Not Permitted in Edit Mode"); return; }
            

        color = c;
        emissivity = em;
        if (colorsinuse.ContainsKey(c))
            m = colorsinuse[color];
        else
        {
            m = NewMaterial(color, emissivity);
            colorsinuse.Add(color, m);
        }
        target.GetComponent<MeshRenderer>().material = m;
        currentColor = color;

    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            target = gameObject;
        if (color != currentColor)                                      //if in edit mode just change material properties, if in play mode use batching
        {
            if (Application.isPlaying)
                ChangeColor(color, emissivity);
            else
            {
                m.SetColor("_BaseColor", color);
                m.SetColor("_EmissionColor", color * emissivity);
                currentColor = color;
            }
        }
    }
    //an improvement to this approach would be to store a count of references to each material in the dict and delete the materials when they are unreferenced.
    //also emission isnt updated at runtime until color is changed.
}
