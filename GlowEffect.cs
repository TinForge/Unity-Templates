using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Glow emmision when object touches trigger
/// </summary>
public class GlowEffect : MonoBehaviour
{
    private MeshRenderer mr;
    public Color color;
    private bool glowing;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    public void ToggleGlow(bool toggle)
    {
        if (toggle)
            StartCoroutine(GlowOn());
        else
            StartCoroutine(GlowOff());
    }

    private IEnumerator GlowOn()
    {
        float t = 0;
        while (t < 1)
        {
            float a = Mathf.Lerp(-4, 1, t);
           
            mr.material.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(a));
            t += Time.deltaTime * 2;
            yield return null;
        }
    }

    private IEnumerator GlowOff()
    {
        float t = 0;
        while (t < 1)
        {
            float a = Mathf.Lerp(1, -4, t);

            mr.material.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(a));
            t += Time.deltaTime * 2;
            yield return null;
        }
    }
}
