using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricityCurve : MonoBehaviour
{
    VisualEffect electricity;
    public Vector3 target;
    public float randomScale;
    float cooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        electricity = GetComponent<VisualEffect>();
        electricity.SetVector3("Bezier 1", transform.position);
        electricity.SetVector3("Bezier 4", target);
    }

    private void Update()
    {
        if(cooldown<0)
        { 
            float t2 = Random.Range(.2f, .45f);
            float t3 = Random.Range(.55f, .80f);
            Vector3 bezier2base = Vector3.Lerp(transform.position, target, t2);
            Vector3 bezier3base = Vector3.Lerp(transform.position, target, t3);
            Vector3 random2 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * randomScale;
            Vector3 random3 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * randomScale;

            electricity.SetVector3("Bezier 1", transform.position);
            electricity.SetVector3("Bezier 2", bezier2base + random2);
            electricity.SetVector3("Bezier 3", bezier3base + random3);
            electricity.SetVector3("Bezier 4", target);
            cooldown = Random.Range(.3f, .8f);
        }
        cooldown -= Time.deltaTime;
    }

}
