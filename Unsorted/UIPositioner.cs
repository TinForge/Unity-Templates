using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goes on UI elements that need to be positioned at a transform target.
/// Use lerping to achieve a faux size scaling affect
/// Since it sets the position closer to the camera, DynamicPositioning will clash with how UIFader interprets distance to target
/// </summary>
public class UIPositioner : MonoBehaviour
{
    [SerializeField] private Transform anchor;

    [Tooltip("Camera needs to be in the scene to work")]
    [SerializeField] private bool dynamicPositioning = true;
    private Transform target;

    private const float minLerp = 0;
    private const float maxLerp = 0.5f;
    private const float minDistance = 2;
    private const float maxDistance = 7;


    private float distance;
    private float lerpValue;

    void Awake()
    {
        target = FindObjectOfType<Camera>().transform;
        if (target == null)
            Debug.LogError("DynamicPositioning enabled, but no valid camera found in scene");
    }

    void Update()
    {

        if (dynamicPositioning && anchor != null & target != null)
        {
            ClampLerp();
            ApproachTarget();
        }
        else if (anchor != null)
        {
            transform.position = anchor.position;
        }


    }

    void ClampLerp()
    {
        distance = Vector3.Distance(anchor.position, target.position);
        lerpValue = Mathf.InverseLerp(minDistance, maxDistance, distance);
        lerpValue = Mathf.Clamp(lerpValue, minLerp, maxLerp);
    }

    void ApproachTarget()
    {
        transform.position = Vector3.Lerp(anchor.position, target.position, lerpValue);
    }



}
