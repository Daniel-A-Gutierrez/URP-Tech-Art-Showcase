using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;
    public float speed = 2.0f;
    private float yaw = 0f;
    private float pitch = 0f;

    void LateUpdate()
    {
        //Look around with Right Mouse
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeedH * Input.GetAxis("Mouse X");
            pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        //drag camera around with Middle Mouse
        if (Input.GetMouseButton(2))
        {
            transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
        }

        //Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);

        Vector3 movedir =
            ((Input.GetKey(KeyCode.E) ? transform.up : Vector3.zero) +
            (Input.GetKey(KeyCode.Q) ? -transform.up : Vector3.zero) +
            (Input.GetKey(KeyCode.A) ? -transform.right : Vector3.zero) +
            (Input.GetKey(KeyCode.S) ? -transform.forward : Vector3.zero) +
            (Input.GetKey(KeyCode.W) ? transform.forward : Vector3.zero) +
            (Input.GetKey(KeyCode.D) ? transform.right : Vector3.zero)) *
            (Input.GetKey(KeyCode.LeftShift) ? 3.0f : 1.0f) * speed;

        transform.Translate(movedir * Time.deltaTime, Space.World);


    }
}