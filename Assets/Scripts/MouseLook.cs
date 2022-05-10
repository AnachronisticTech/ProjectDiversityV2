using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField, Range(1.0f, 10.0f)]
    private float mouseSensitivity = 8.0f;
    private const float mouseSensOffset = 100.0f;

    private Transform player;

    private float xRotation;
    private float mouseX, mouseY;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * mouseSensOffset * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * mouseSensOffset * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 80.0f);

        transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        player.Rotate(Vector3.up * mouseX);
    }
}
