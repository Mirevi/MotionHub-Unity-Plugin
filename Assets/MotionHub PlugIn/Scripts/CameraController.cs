using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float roatationSensitivity = 0.05f;
    public float zoomSensitivity = 0.5f;

    public GameObject pivot;

    Vector3 dragStart;

    Vector3 startPosition;
    Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = pivot.transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            dragStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            pivot.transform.rotation = Quaternion.Euler(Vector3.up * (Input.mousePosition.x - dragStart.x) * roatationSensitivity + pivot.transform.rotation.eulerAngles);
            pivot.transform.rotation = Quaternion.Euler(Vector3.right * (Input.mousePosition.y - dragStart.y) * roatationSensitivity + pivot.transform.rotation.eulerAngles);

            dragStart = Input.mousePosition;
        }

        transform.position += (transform.position - pivot.transform.position) * Input.mouseScrollDelta.y * zoomSensitivity;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
        }
    }

    public void Reset()
    {
        transform.localPosition = startPosition;
        pivot.transform.rotation = startRotation;
    }
}
