using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 10f; // speed at which the mouse moves
    public Transform playerBody; 
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to center of screen so that the mouse isn't seen
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity; 
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity; 

        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX); //rotatin accross x axis
    }
}
