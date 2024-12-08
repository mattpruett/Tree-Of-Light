using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    KeyCode moveLeftKey = KeyCode.Q;
    [SerializeField]
    KeyCode moveRightKey = KeyCode.E;

    [SerializeField]
    float movementSpeed;

    void Update()
    {
        if (Input.GetKey(moveLeftKey))
        {
            SwivelCamera(Vector3.left);
        }
        else if (Input.GetKey(moveRightKey))
        {
            SwivelCamera(Vector3.right);
        }
    }

    private void SwivelCamera(Vector3 direction)
    {
        //transform.LookAt(target);
        //transform.Translate(direction * Time.deltaTime * movementSpeed);
    }
}
