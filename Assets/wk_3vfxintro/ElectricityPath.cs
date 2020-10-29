using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class ElectricityPath : MonoBehaviour
{
    VisualEffect lightningvfx;
    public Vector3 target;
    public float randomScale;
    public bool isstatic = true;
    float countdown;
    void Start()
    {
        lightningvfx = GetComponent<VisualEffect>();
        lightningvfx.SetVector3("bezier 1", transform.position);
        lightningvfx.SetVector3("bezier 4", target);
        StartCoroutine("ChangePath");
    }

    // Update is called once per frame
    void Update()
    {
        if(!isstatic)
        {
            lightningvfx.SetVector3("bezier 1", transform.position);
            lightningvfx.SetVector3("bezier 4", target);
        }
        if(countdown<=0)
        { 
            Vector3 b2 = Vector3.Lerp(transform.position, target, Random.Range(.25f, .45f));
            Vector3 b3 = Vector3.Lerp(transform.position, target, Random.Range(.55f, .95f));
            Vector3 r1 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-.25f, 2.0f), Random.Range(-1.0f, 1.0f));
            Vector3 r2 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-2.0f, 2.0f), Random.Range(-1.0f, 1.0f));
            b2 += r1 * randomScale;
            b3 += r2 * randomScale;
            lightningvfx.SetVector3("bezier 2", b2);
            lightningvfx.SetVector3("bezier 3", b3);
            countdown = Random.Range(.15f, .5f);
        }
        countdown -= Time.deltaTime;
    }

    IEnumerable ChangePath()
    {
        while (true) 
        { 
            Vector3 b2 = Vector3.Lerp(transform.position, target, Random.Range(.25f, .45f));
            Vector3 b3 = Vector3.Lerp(transform.position, target, Random.Range(.55f, .95f));
            Vector3 r1 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            Vector3 r2 = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            b2 += r1 * randomScale;
            b3 += r2 * randomScale;
            lightningvfx.SetVector3("bezier 2", b2);
            lightningvfx.SetVector3("bezier 3", b3);
            print("Its running");
            yield return new WaitForSeconds(Random.Range(.3f, .8f));
        }
    }
}
