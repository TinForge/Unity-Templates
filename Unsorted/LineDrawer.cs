using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    private LineRenderer line;
    private CanvasGroup cg;
    private Color alphaColor;

    [SerializeField] private Transform target;

    [Tooltip("Optional")]
    [SerializeField] private RectTransform attachedPanel;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        alphaColor = line.material.color;
        cg = attachedPanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }

    void Update()
    {
        line.SetPosition(1, target.position);

        if (attachedPanel != null)
        {
            Vector3[] corners = new Vector3[4];
            attachedPanel.GetWorldCorners(corners);
            line.SetPosition(0, (corners[0] + corners[3]) / 2);
            alphaColor.a = cg.alpha;
            line.material.color = alphaColor;
        }
        else
            line.SetPosition(0, transform.position);
    }
}
