using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera Controller to test scenes quickly
/// </summary>
public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private float zMove;
    [SerializeField] private float xMove;

    [SerializeField] private float yLook;
    [SerializeField] private float xLook;

    [SerializeField] private bool stop;

    private void Awake()
    {
        if (GetComponentInChildren<Camera>() == null)
            Debug.LogError("Camera not found for SimpleCameraController");
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            stop = !stop;
        if (stop)
            return;

        float zM = zMove * Input.GetAxis("Vertical") * Time.deltaTime;
        float xM = xMove * Input.GetAxis("Horizontal") * Time.deltaTime;
        transform.position += transform.right * xM + transform.forward * zM;

        float yL = (yLook  * Time.deltaTime) * Input.GetAxis("Mouse Y");
        float xL = (xLook  * Time.deltaTime) * Input.GetAxis("Mouse X");
        Vector3 rot = transform.rotation.eulerAngles;
        transform.Rotate(new Vector3(-yL, xL));
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}
