using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds and looks at a camera.
/// Can pivot on y-axis only, smoothing, and min distance
/// </summary>
public class LookAtCamera : MonoBehaviour
{

    private Transform target;

    [Tooltip("Necessary for non-UI elements that should face the target on the +Z axis")]
    [SerializeField] private bool invertLookDirection;
    [SerializeField] private bool yAxis;
    [SerializeField] private bool useLerp = true;
    private const float lerpAmount = 0.1f;

    [Tooltip("Prevent erratic rotations when target is too close, mostly for y-axis pivots")]
    [SerializeField] private float minDistance = 2;


    void Start()
    {
        target = FindObjectOfType<Camera>().transform;
    }

    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) < minDistance)
            return;

        Quaternion oldRot = transform.rotation;

        if (invertLookDirection)
            transform.LookAt(target);
        else
            transform.rotation = Quaternion.LookRotation(transform.position - target.position);

        if (yAxis)
        {
            Quaternion rot = transform.rotation;
            transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        }

        if (useLerp)
            transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, lerpAmount);

    }
}
