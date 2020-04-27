using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fade animation for spawning/despawning object
/// </summary>
public class FadeEffect : MonoBehaviour
{
    private Rigidbody rb;
    private MeshRenderer mr;

    [SerializeField] private Material opaque;
    [SerializeField] private Material fade;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(In());
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Out());
    }

    public IEnumerator In()
    {
        mr.material = fade;
        Color color = mr.material.color;
        rb.isKinematic = true;
        
        float t = 0;
        while (t < 1) //Scale in
        {
            color.a = t;
            mr.material.color = color;
            t += Time.deltaTime * 2;
            yield return null;
        }
        mr.material = opaque;
        rb.isKinematic = false;
    }

    public IEnumerator Out()
    {
        Color color = mr.material.color;
        Vector3 scale = transform.localScale;

        float t = 0;
        while (t < 1) //Scale out
        {
            color.a = 1 - t;
            mr.material.color = color;
            t += Time.deltaTime * 2;
            yield return null;
        }
        color.a = 0;
        mr.material.color = color;

        gameObject.SetActive(false);
    }


}
