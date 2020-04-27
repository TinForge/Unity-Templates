using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Static singleton to decouple audio and component referencing.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioSource showcaseSwitch;
    public static AudioSource ShowcaseSwitch;

    [SerializeField] private AudioSource teleport;
    public static AudioSource Teleport;

    [SerializeField] private AudioSource pointerDown;
    public static AudioSource PointerDown;

    [SerializeField] private AudioSource pointerUp;
    public static AudioSource PointerUp;

    [SerializeField] private AudioSource pointerEnter;
    public static AudioSource PointerEnter;

    [SerializeField] private AudioSource pointerExit;
    public static AudioSource PointerExit;


    void Awake()
    {
        instance = this;

        ShowcaseSwitch = showcaseSwitch;
        Teleport = teleport;
        PointerDown = pointerDown;
        PointerUp = pointerUp;
        PointerEnter = pointerEnter;
        PointerExit = pointerExit;
    }

    public static void PlaySound(AudioSource sound)
    {
        sound.Play();
    }
}
