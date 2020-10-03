using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotate : MonoBehaviour
{
    //goals : First print the rotation between two vectors
    //then, rotate one vector to the other
    //then, do it slowly
    //bonus : generate random quaternions.


    public Transform track;
    Rigidbody body;
    Quaternion initial;
    public float slowThreshold=10.0f;
    public float maxAnglePerSecond;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        initial = body.rotation;
        print(Quaternion.Angle(Quaternion.identity,Quaternion.FromToRotation(transform.forward, (track.position - transform.position).normalized)));
    }

    // Update is called once per frame
    void Update()
    {
        UnconstrainedRotate(track.position);

    }

    void UnconstrainedRotate(Vector3 target)
    {
        Quaternion fullRotation = Quaternion.FromToRotation(transform.forward, (target - transform.position).normalized);//rotation from fwd to trgt
        float angle = Quaternion.Angle(Quaternion.identity,fullRotation);
        //if we're close slow down, if we're above an arbitrary threshold full speed.
        //if angle is less than 10 degrees, speed *= degrees/threshold ^.5 
        //in other words, speed*=  sqrt ( min ( degrees/threshold , 1  ) ) 
        Quaternion toRotation = fullRotation*body.rotation;
        float turn = (maxAnglePerSecond*Time.deltaTime)*Mathf.Sqrt(Mathf.Min(angle/slowThreshold,1.0f));
        Quaternion delta = Quaternion.RotateTowards(body.rotation,toRotation,turn);
        body.MoveRotation(delta);
    }

    void ConstrainedRotate(Vector3 axis)
    {
        axis = axis.normalized;
        Plane projection = new Plane(axis, -Vector3.Dot(transform.position,axis));                                                        //create plane
        //projection.Translate(projection.normal*projection.GetDistanceToPoint(transform.position));       //translate till its on the game object
        Vector3 closest = projection.ClosestPointOnPlane(track.position);                               //get closest point on plane of target
        print(projection.GetDistanceToPoint(closest));
        UnconstrainedRotate(closest);

    }
}
