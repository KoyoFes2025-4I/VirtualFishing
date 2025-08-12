using System;
using UnityEngine;

public class ConfigCameraScript : MonoBehaviour
{
    [SerializeField]
    GameObject configCamera;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float dashSpeed = 15f;
    [SerializeField]
    private float mouseSensitivity = 500f;
    private bool locked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) locked = !locked;
        if (locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;

        if (locked) Move();
    }

    private float xRotation = 0f;
    private void Move()
    {
        Vector3 delta = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) delta += transform.forward;
        if (Input.GetKey(KeyCode.A)) delta -= transform.right;
        if (Input.GetKey(KeyCode.S)) delta -= transform.forward;
        if (Input.GetKey(KeyCode.D)) delta += transform.right;
        if (Input.GetKey(KeyCode.E)) delta += transform.up;
        if (Input.GetKey(KeyCode.Q)) delta -= transform.up;
        if (delta.magnitude != 0) delta.Normalize();

        if (Input.GetKey(KeyCode.LeftShift)) delta *= dashSpeed;
        else delta *= speed;

        transform.position += delta * Time.deltaTime;



        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        configCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
