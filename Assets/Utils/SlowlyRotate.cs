using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyRotate : MonoBehaviour
{
    public Vector3 axis;
    public bool clockwise;
    public float period;
    private float rotationAngle;
    private Quaternion initialRotation;
    void Start()
    {
        float sra2 = Mathf.Sin(rotationAngle / 2.0f);
        //initialRotation = new Quaternion(sra2 * transform.up.x, sra2 * transform.up.y, sra2 * transform.up.z, 1);
        initialRotation = GetComponent<Rigidbody>().rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // RotationAngle is in radians
        float sra2 = Mathf.Sin(rotationAngle / 2.0f);
        float x = axis.x * sra2;
        float y = axis.y * sra2;
        float z = axis.z * sra2;
        float w = Mathf.Cos(rotationAngle / 2.0f);
        Quaternion q = new Quaternion(x, y, z, w);
        GetComponent<Rigidbody>().MoveRotation(q*initialRotation);
        rotationAngle += Time.deltaTime / period * Mathf.PI*2;
        //transform.Rotate(axis, Time.deltaTime/period *(clockwise? 1:-1)*Mathf.Rad2Deg ,Space.World); //works, but no friction 
    }
}
