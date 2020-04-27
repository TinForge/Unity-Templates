using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ping pong position/rotation for dynamic scenery
/// </summary>
public class RepetitiveMotion : MonoBehaviour
{
    /*
    [SerializeField] private float posTime = 1;
    [SerializeField] private float posX;
    [SerializeField] private float posY;
    [SerializeField] private float posZ;
    */

    [SerializeField] private float rotTime = 1;
    [SerializeField] private float rotX;
    [SerializeField] private float rotY;
    [SerializeField] private float rotZ;

    //private Vector3 posOffset;
    private Vector3 rotOffset;

    void Start()
    {
        //posOffset = transform.position;
        rotOffset = transform.eulerAngles;
    }

    void Update()
    {
        /*
        float pX = Mathf.PingPong(Time.unscaledTime * (posTime * 1f), posX * 2) - posX;
        float pY = Mathf.PingPong(Time.unscaledTime * (posTime * 0.9f), posY * 2) - posY;
        float pZ = Mathf.PingPong(Time.unscaledTime * (posTime * 0.8f), posZ * 2) - posZ;
        */

        float rX = Mathf.PingPong(Time.unscaledTime * rotTime, rotX * 2) - rotX;
        float rY = Mathf.PingPong(Time.unscaledTime * rotTime, rotY * 2) - rotY;
        float rZ = Mathf.PingPong(Time.unscaledTime * rotTime, rotZ * 2) - rotZ;

        /*
        if (pX != 0 || pY != 0 || pZ != 0)
            transform.position = posOffset + new Vector3(pX, pY, pZ);
        */

        if (rX != 0 || rY != 0 || rZ != 0) //if any equals 0, throws a quaternion error
            transform.eulerAngles = rotOffset + new Vector3(rX, rY, rZ);

    }
}
