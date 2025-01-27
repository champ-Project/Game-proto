using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;

public enum SlideDirection
{
    vertical,
    horizontal
}

public class SlideBlock : MonoBehaviour
{
    private Plane dragPlane;
    private float planeDistance = 0f;
    [SerializeField] private SlideDirection slideDirection;
    private Vector3 clickOffset = Vector3.zero;
    [SerializeField] bool isSelect = false;
    [SerializeField] private CinemachineCamera nowCamera;

    private float distance;
    private Vector3 offset;

    //private float forwardLimit;
    //private float backwardLimit;

    private float moveDistance = 5f;
    private Vector3 forwardLimit = Vector3.positiveInfinity;
    private Vector3 backwardLimit = Vector3.negativeInfinity;

    private Vector3 mouseDownPos;
    private BoxCollider thisCol;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    float smoothTime = 0.1f;
    private Vector3 originPos;

    private Vector3 nowTargetPos;
    [SerializeField] private Transform nowTargetTrans;
    [SerializeField] private BoxCollider[] colliders = new BoxCollider[2];

    private void Start()
    {
        dragPlane = new Plane(Vector3.up, transform.position);
        thisCol = GetComponent<BoxCollider>();
        targetPosition = transform.position;
        nowTargetPos = Vector3.zero;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("SlideSocket") == true)
        {
            BoxCollider otherCol = other.GetComponent<BoxCollider>();
            if (otherCol != null)
            {
                for(int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] == null)
                    {
                        colliders[i] = otherCol;
                        break;
                    }
                }
            }



            nowTargetPos = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            
            nowTargetTrans = other.gameObject.transform;
        }
    }

    private void OnMouseDown()
    {
        originPos = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }

        /*distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        offset = transform.position - Camera.main.ScreenToWorldPoint(mousePos);

        CalculateMoveLimits();*/

        GetMoveRange();
    }

    private void GetMoveRange()
    {
        float halfSize = transform.localScale.z / 2f;

        //Ray forwardRay = new Ray(transform.position, transform.forward);
        //Ray backwardRay = new Ray(transform.position, -transform.forward);

        Ray ray = new Ray(); //오브젝트 체크를 위한 Ray
        ray.origin = transform.position;
        RaycastHit hit;
        LayerMask slideWallMask = LayerMask.GetMask("SlideWall");
        thisCol.enabled = false;

        ray.direction = transform.forward;
        if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, slideWallMask))
        {
            forwardLimit = hit.point - transform.forward * halfSize;    
        }

        ray.direction = -transform.forward;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, slideWallMask))
        {
            backwardLimit = hit.point + transform.forward * halfSize;
        }

        /*Ray forwardRay = new Ray(transform.position + transform.forward * halfSize, transform.forward);
        Ray backwardRay = new Ray(transform.position - transform.forward * halfSize, -transform.forward);

        RaycastHit forwardHit;
        if (Physics.Raycast(forwardRay, out forwardHit, moveDistance, slideWallMask))
        {
            Debug.Log(forwardHit.collider.gameObject.name);
            //forwardLimit = forwardHit.point;
            forwardLimit = forwardHit.point - transform.forward * halfSize; // 앞쪽 충돌 지점
        }
        else
        {
            //forwardLimit = transform.position + transform.forward * moveDistance; // 최대 거리
            forwardLimit = transform.position + transform.forward * (moveDistance - halfSize); // 최대 거리
        }

        RaycastHit backwardHit;
        if (Physics.Raycast(backwardRay, out backwardHit, moveDistance, slideWallMask))
        {
            Debug.Log(backwardHit.collider.gameObject.name);
            //backwardLimit = backwardHit.point;
            backwardLimit = backwardHit.point + transform.forward * halfSize; // 뒤쪽 충돌 지점
        }
        else
        {
            //backwardLimit = transform.position - transform.forward * moveDistance; // 최대 거리
            backwardLimit = transform.position - transform.forward * (moveDistance - halfSize); // 최대 거리
        }*/

        thisCol.enabled = true;
    }

    private void OnMouseUp()
    {
        isSelect = false;
        if(nowTargetTrans != null)
        {
            targetPosition = nowTargetPos;
        }
        /*else
        {
            transform.position = originPos;
        }*/
    }

    void OnMouseDrag()
    {

        /*float distance = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);

        //objPos.z = 0;
        objPos.x = 0;
        transform.position = objPos;*/

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        /*Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if(slideDirection == SlideDirection.horizontal)
        {
            transform.position = new Vector3(worldPos.x + offset.x, transform.position.y, transform.position.z);
        }
        else if(slideDirection == SlideDirection.vertical)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, worldPos.z + offset.z);
        }*/

        /*Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 현재 오브젝트의 forward 방향으로 이동
        float newZ = transform.localPosition.z + (worldPos.z - transform.position.z);

        // 앞뒤 이동 제한
        if (newZ >= transform.localPosition.z - backwardLimit && newZ <= transform.localPosition.z + forwardLimit)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
        }*/

        /*Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.forward );*/

        // 현재 오브젝트의 forward 방향으로 이동
        //Vector3 direction = transform.forward * (worldPos.z - transform.position.z);
        //float newZ = transform.position.z + direction.z;

        //transform.position = new Vector3(transform.position.x, transform.position.y, newZ);

        // 앞뒤 이동 제한
        /*if (newZ >= transform.position.z - backwardLimit && newZ <= transform.position.z + forwardLimit)
        {
            
        }*/

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;

        /*if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 newPosition = hitPoint + offset;

            // transform.forward 방향으로만 이동하도록 제한합니다.

            

            Vector3 forwardMovement = Vector3.Project(newPosition - transform.position, transform.forward);

            if (transform.position.x > forwardMovement.x)
            {
                
            }
                transform.position += forwardMovement;


            if ((newPosition.z > backwardLimit.z && newPosition.z < forwardLimit.z) || (newPosition.x > backwardLimit.x && newPosition.x < forwardLimit.x))
            {
                
            }
            
        }*/

        if(dragPlane.Raycast(ray, out enter))
        {
            MoveSlideBlock(ray.GetPoint(enter));
        }
    }

    private void MoveSlideBlock(Vector3 screenPosition)
    {
        Vector3 offsetPos = screenPosition + offset;

        switch (slideDirection)
        {
            case SlideDirection.vertical:
                offsetPos.x = transform.position.x;
                offsetPos.z = Mathf.Clamp(offsetPos.z, backwardLimit.z, forwardLimit.z);
                break; 
            case SlideDirection.horizontal:
                offsetPos.z = transform.position.z;
                offsetPos.x = Mathf.Clamp(offsetPos.x, backwardLimit.x, forwardLimit.x);
                break;
        }

        targetPosition = offsetPos;
    }

    private void CalculateMoveLimits()
    {
        /*Ray ray;
        RaycastHit hit;

        // forward 방향으로 ray 쏘기
        ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            forwardLimit = hit.distance; // 앞쪽으로의 거리
        }
        else
        {
            forwardLimit = Mathf.Infinity; // 감지된 오브젝트가 없으면 무한대
        }

        // backward 방향으로 ray 쏘기
        ray = new Ray(transform.position, -transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            backwardLimit = hit.distance; // 뒤쪽으로의 거리
        }
        else
        {
            backwardLimit = Mathf.Infinity; // 감지된 오브젝트가 없으면 무한대
        }*/
    }
}
