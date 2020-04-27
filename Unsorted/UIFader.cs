using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fades UI Elements by distance or position in viewport.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    private CanvasGroup cg;
    private Camera cam;

    //

    [SerializeField] private bool useDistance = true;
    [SerializeField] private float fadeInDistance = 2;
    [SerializeField] private float fadeOutDistance = 5; 

    private float distance;
    private float distanceAlpha;

    //

    [Space]
    [SerializeField] private bool useViewport = true;
    [Range(0, 1)]
    [Tooltip("1 = Narrow Focus, 0 = Wide Focus")]
    [SerializeField]
    private float bounds = 0.5f;
    private float minBounds {get {return bounds/2; } }
    private float maxBounds { get { return 1 - minBounds; } }
    private const float Center = 0.5f;

    private Vector3 viewPos;
    private float viewAlpha;

    [Space]
    [Tooltip("Optional for a second target point")]
    [SerializeField] private Transform target;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        cam = FindObjectOfType<Camera>();
        if (bounds == 0)
            bounds = 0.01f;
    }

    void Update()
    {
        if (useDistance)
            DistanceFade();

        if (useViewport)
            if (target == null)
                viewAlpha = ViewportFade(transform);
            else
                viewAlpha = Mathf.Max(ViewportFade(transform), ViewportFade(target));

        cg.alpha = Mathf.Lerp(cg.alpha, viewAlpha, 0.1f);

        if (useDistance && useViewport)
            LerpAlphas();
    }

    private void DistanceFade()
    {
        float distanceLerp;

        distance = Vector3.Distance(transform.position, cam.transform.position);
        distanceLerp = Mathf.InverseLerp(fadeOutDistance, fadeInDistance, distance);
        distanceAlpha = Mathf.Lerp(0f, 1, distanceLerp);

        cg.alpha = Mathf.Lerp(cg.alpha, distanceAlpha, 0.1f);
    }

    private float ViewportFade(Transform t)
    {
        float viewLerp;

        viewPos = cam.WorldToViewportPoint(t.position);
        if (viewPos.x >= minBounds && viewPos.x <= maxBounds && viewPos.y >= minBounds && viewPos.y <= maxBounds) //Checks if in center sweetspot of viewport
            viewLerp = 1;
        else if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1) //Checks if in the viewport
        {
            if (viewPos.x > Center) //Converts upper bounds to lower bounds, to match alpha
                viewPos.x = Mathf.Abs(viewPos.x - 1);
            if (viewPos.y > Center)
                viewPos.y = Mathf.Abs(viewPos.y - 1);
            viewLerp = Mathf.Min(viewPos.x, viewPos.y);
        }
        else //not in the viewport
            viewLerp = 0;

        viewLerp = Mathf.InverseLerp(0, minBounds, viewLerp);
        viewLerp = Mathf.Lerp(0f, 1, viewLerp);
        return viewLerp;
    }
    
    private void LerpAlphas()
    {
        cg.alpha = (distanceAlpha * distanceAlpha) * (viewAlpha * viewAlpha);
        //cg.alpha = Mathf.Lerp(distanceAlpha, viewAlpha, 0.5f);
    }

}
