using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    public GameObject followObject;
    public float smoothTime = 0.3f;
    Vector3 moveVel;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, followObject.transform.position + offset, ref moveVel, smoothTime);
    }
}
