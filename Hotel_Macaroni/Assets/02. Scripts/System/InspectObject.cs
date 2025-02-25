using System.Runtime.InteropServices;
using UnityEngine;

public class InspectObject : MonoBehaviour
{
    private float rotationSpeed = 30f;
    private Vector3 prevMousePos;

    private void OnMouseDown()
    {
        prevMousePos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector3 deltaMousePos = Input.mousePosition - prevMousePos;
        float rotationX = deltaMousePos.y * rotationSpeed * Time.deltaTime;
        float rotationY = -deltaMousePos.x * rotationSpeed * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        this.transform.rotation = rotation * this.transform.rotation;

        prevMousePos = Input.mousePosition;
    }
}
