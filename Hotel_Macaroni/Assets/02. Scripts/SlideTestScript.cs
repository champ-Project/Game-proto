using UnityEngine;

public class SlideTestScript : MonoBehaviour
{
    float moveRange = 0.5f;

    private void Start()
    {
        
    }
    private void OnMouseDown()
    {
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = transform.right;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        //transform.x 이하 동일인 경우 로컬방향으로 y축
        //transform.y + moveRange => 로컬 방향으론 x(씬 창 기준)이지만, transform컴포넌트 기준 y축 올라감

        // 실제이동 - 월드 x, 오브젝트 transform기준 - y축 값 변함
        //월드 좌표 x축으로 이동함 오브젝트 회전값과 관계없이 => transform.position이어야만 함 (localPosition하면 이상해짐)
        //오브젝트의 transform컴포넌트 기준으론 y축 값이 변경됨 (로테이션 0,0,0 기준으로 y방향이 world position X방향임
        Vector3 targetPosition = new Vector3(transform.position.x + moveRange, transform.position.y, transform.position.z);

        //실제 이동 - 월드 y, 오브젝트 transform기준 - x축 값 변함
        //로컬 좌표 x축 값이 변함(로테이션 0,0,0 기준...) 정확히는 해당 오브젝트의 transform컴포넌트의 x축으로 움직임
        //오브젝트는 위 방향(월드 y로 움직임)
        Vector3 localTargetPosition = new Vector3(transform.localPosition.x + moveRange, transform.localPosition.y, transform.localPosition.z);

        //실제 이동 - 월드 y, 오브젝트 transform기준 - x축 값 변함
        //로컬 방향 위 방향으로 움직임 (로컬x) 이것도 로테이션 영향 x
        Vector3 localTargetPosition2 = transform.localPosition - transform.up * moveRange;

        //실제 이동 - 월드 x, 오브젝트 transform기준 - y축 값 변함
        //월드 좌표 x축, 로컬 y축이 움직임.. 위 방식과 다른점은 
        Vector3 targetPosition2 = transform.forward * moveRange;


        //Vector3 localTagetPosTest2 = new Vector3(transform)
        //this.transform.localPosition = localTargetPosition2;
        //this.transform.position = targetPosition2;

        Vector3 moveDirection = transform.right;
        transform.position += moveDirection * moveRange;
    }
}
