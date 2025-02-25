using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum SlideDirection
{
    vertical,
    horizontal
}

public class SlideBlock : MonoBehaviour
{
    private Plane dragPlane;
    [SerializeField] private SlideDirection slideDirection;
    [SerializeField] [Range(2, 3)]private int boxSize;
    [SerializeField] bool isSelect = false;
    [SerializeField] private CinemachineCamera nowCamera;

    private Vector3 offset;
    private Vector3 forwardLimit = Vector3.positiveInfinity;
    private Vector3 backwardLimit = Vector3.negativeInfinity;
    private BoxCollider thisCol;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    float smoothTime = 0.1f;
    private Vector3 originPos;

    //private Vector3 nowTargetPos;
    //[SerializeField] private Transform nowTargetTrans;
    [SerializeField] private List<Collider> socketCols = new List<Collider>();

    private void Start()
    {
        dragPlane = new Plane(transform.up, transform.position);
        thisCol = GetComponent<BoxCollider>();
        targetPosition = transform.position;
        //nowTargetPos = Vector3.zero;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag("SlideSocket") == true)
        {
            if (!socketCols.Contains(other))
            {
                socketCols.Add(other);
                //Debug.Log("트리거반응");
            }

            //nowTargetPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            
            //nowTargetTrans = other.gameObject.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (socketCols.Contains(other))
        {
            socketCols.Remove(other);
        }
    }

    private void OnMouseDown()
    {
        //Debug.Log(this.name + "클릭");
        originPos = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }
        GetMoveRange();
    }

    private void GetMoveRange()
    {
        float halfSize = transform.localScale.z / 2f;

        Ray ray = new Ray(); //오브젝트 체크를 위한 Ray
        ray.origin = transform.position;
        RaycastHit hit;
        LayerMask slideWallMask = LayerMask.GetMask("SlideWall");
        thisCol.enabled = false;

        //Vector3 thisDirection = (slideDirection == SlideDirection.horizontal) ? Vector3.up : transform.forward;

        ray.direction = transform.forward;
        if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, slideWallMask))
        {
            //Debug.Log(hit.collider.name + "체크1");
            forwardLimit = hit.point - transform.forward * halfSize;    
        }

        ray.direction = -transform.forward;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, slideWallMask))
        {
            //Debug.Log(hit.collider.name+ "체크2");
            backwardLimit = hit.point + transform.forward * halfSize;
        }

        thisCol.enabled = true;
    }

    private void OnMouseUp()
    {
        isSelect = false;

        if (socketCols.Count < 2)
        {
            targetPosition = originPos;
            return;
        }

        if(boxSize == 2)
        {
            Collider col1 = null;
            Collider col2 = null;

            float colDistance1 = float.MaxValue;
            float colDistance2 = float.MaxValue;

            foreach (var col in socketCols)
            {
                Vector3 colCenter = col.bounds.center;

                float distance = Vector3.Distance(transform.position, colCenter);

                if (distance < colDistance1)
                {
                    colDistance2 = colDistance1;
                    col2 = col1;
                    colDistance1 = distance;
                    col1 = col;
                }
                else if (distance < colDistance2)
                {
                    colDistance2 = distance;
                    col2 = col;
                }
            }

            if (col1 != null && col2 != null)
            {
                Vector3 newPos = (col1.bounds.center + col2.bounds.center) / 2;
                //newPos.y = transform.position.y;
                targetPosition = newPos;
            }
        }
        else if (boxSize == 3)
        {
            Collider col1 = null;
            float colDistance1 = float.MaxValue;

            foreach (var col in socketCols)
            {
                Vector3 colCenter = col.bounds.center;
                float distance = Vector3.Distance(transform.position, colCenter);

                if(distance < colDistance1)
                {
                    colDistance1 = distance;
                    col1 = col;
                }
            }

            if(col1 != null)
            {
                Vector3 newPos = col1.bounds.center;
                //newPos.y = transform.position.y;
                targetPosition = newPos;
            }
        }
    }

    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;

        if(dragPlane.Raycast(ray, out enter))
        {
            MoveSlideBlock(ray.GetPoint(enter));
        }
    }

    private void MoveSlideBlock(Vector3 screenPosition)
    {
        Vector3 offsetPos = screenPosition + offset;

        //
        Vector3 localOffsetPos = transform.InverseTransformPoint(offsetPos);

        switch (slideDirection)
        {
            case SlideDirection.vertical:
                offsetPos.x = transform.position.x;
                offsetPos.y = transform.position.y;
                offsetPos.z = Mathf.Clamp(offsetPos.z, backwardLimit.z, forwardLimit.z);
                targetPosition = offsetPos;


                break; 
            case SlideDirection.horizontal:
                offsetPos.z = transform.position.z;
                offsetPos.y = Mathf.Clamp(offsetPos.y, backwardLimit.y, forwardLimit.y);
                targetPosition = offsetPos;


                break;
        }

        
        //targetPosition = offsetPos;
    }
}
