using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamaraController : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private Gravity SelectedObject = null;
    [SerializeField]
    private float Zoom = 10.0f;
    [SerializeField]
    private float Sensitivity = 20.0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var info) && info.transform.TryGetComponent<Gravity>(out var obj))
                SelectedObject = obj;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(1))
        {
            transform.position -= Input.GetAxisRaw("Mouse X") * Sensitivity * transform.right;
            transform.position -= Input.GetAxisRaw("Mouse Y") * Sensitivity * transform.up;
        }

        if (SelectedObject != null)
        {
            Zoom -= Input.mouseScrollDelta.y * 2;
            Zoom = Mathf.Max(Zoom, 1.0f);

            float radius = Mathf.Max(SelectedObject.transform.localScale.x, SelectedObject.transform.localScale.y, SelectedObject.transform.localScale.z);
            transform.rotation = Quaternion.LookRotation(SelectedObject.transform.position - transform.position);
            transform.position = SelectedObject.transform.position - transform.forward * (radius + Zoom);
        }
    }
}
