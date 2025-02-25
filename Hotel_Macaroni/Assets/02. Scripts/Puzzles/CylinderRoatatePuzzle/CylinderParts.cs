using UnityEngine;

public class CylinderParts : MonoBehaviour
{
    private float rotationSpeed = 30f;
    private Vector3 prevMousePos;

    [SerializeField] private CylinderPuzzle cylinderPuzzle;
    [SerializeField] private float snapAngle = 36f;
    private float targetAngle;

    private void OnMouseDown()
    {
        prevMousePos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector3 deltaMousePos = Input.mousePosition - prevMousePos;
        //float rotationX = deltaMousePos.y * rotationSpeed * Time.deltaTime;
        float rotationY = -deltaMousePos.x * rotationSpeed * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(0, rotationY, 0);
        this.transform.rotation = rotation * this.transform.rotation;

        prevMousePos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        //
        //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36

        /*// 마우스 버튼을 떼었을 때 호출
        float currentRotation = NormalizeAngle(this.transform.eulerAngles.y); // 현재 Y축 회전값을 정규화
        targetAngle = GetClosestSnapAngle(currentRotation); // 가장 가까운 고정 각도 찾기
        // 실린더를 목표 각도로 즉시 회전
        this.transform.rotation = Quaternion.Euler(0, targetAngle, 0); // 최종 위치 설정*/
        float nowAngle = this.transform.localEulerAngles.y;
        Debug.Log(nowAngle);
        float finalyAngle = GetClosestSnapAngle(nowAngle);
        Debug.Log(finalyAngle);
        //this.transform.localRotation = Quaternion.Euler(this.transform.localRotation.x, finalyAngle, this.transform.localRotation.z);
        //Quaternion rotation = Quaternion.Euler(0, finalyAngle, 0);

        //this.transform.rotation = rotation * this.transform.rotation;
        this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, finalyAngle, this.transform.localEulerAngles.z);

        cylinderPuzzle.CheckPuzzle();
    }

    private float NormalizeAngle(float angle)
    {
        // 각도를 0도에서 360도 범위로 정규화
        while (angle < 0) angle += 360; // 음수일 경우 360을 더함
        while (angle >= 360) angle -= 360; // 360 이상일 경우 360을 뺌
        return angle; // 정규화된 각도를 반환
    }

    private float GetClosestSnapAngle(float angle)
    {
        /*// 스냅 각도를 동적으로 계산
        int numberOfSnapAngles = 10; // 예: 10개의 스냅 각도
        float snapInterval = 360f / numberOfSnapAngles; // 각도 간격 계산

        float closest = 0f; // 초기값
        float smallestDifference = Mathf.Abs(angle - closest); // 초기 최소 차이 계산

        // 동적으로 생성된 스냅 각도 중 가장 가까운 각도 찾기
        for (int i = 0; i <= numberOfSnapAngles; i++)
        {
            float snapAngle = i * snapInterval; // 동적으로 생성된 스냅 각도
            float difference = Mathf.Abs(angle - snapAngle); // 현재 각도와 스냅 각도 간의 차이 계산
            if (difference < smallestDifference) // 현재 차이가 더 작으면
            {
                smallestDifference = difference; // 최소 차이 업데이트
                closest = snapAngle; // 가장 가까운 각도 업데이트
            }
        }

        return closest; // 가장 가까운 스냅 각도를 반환
*/

        // 36 = 스냅 사이의 각도
        // 현재 각도를 36으로 나누면 해당 각도의 스냅을 알 수 있음 ex 735도로 오브젝트가 돌아가면, 735/36 = 20.4111 >> 36x20 = 720
        // 대략적인 스냅(720)이 나오면 해당 스냅의 다음 각도(720+36)(이전 각도는 어짜피 넘어갔으니 계산 할 필요 없음 ex 719 / 36 = 19.972...)
        // 735에서 720을 뺀 값을 = 15 가 36/2보다 작으면 720에, 높으면 720의 다음 스냅각도(720+36)로 하기
        // if 40도로 테스트 => 40 / 36 = 1.111... = 스냅각도는 36x1 = 36도, 다음 스냅 각도는 72도
        // 40도 - 36 = 4가 36/2 보다 작으니 최종 스냅 각도는 36

        float currentY = angle; //현재 y각도
        float calculateDivideY = currentY / snapAngle; //현재 y각도를 스냅 앵글만큼 나눠서 스냅 체크를 위한 배율 값을 구함

        int divideAngle = Mathf.FloorToInt(calculateDivideY); // 배율 값을 int로 변환

        float nowSnapAngle = 36 * divideAngle; //배율 값을 스냅 각도에 곱해서 현재 각도에서 계산할 각도를 구함
        Debug.Log("체크"+nowSnapAngle);
        if((currentY - nowSnapAngle) < (snapAngle / 2)) //만약 현재 각도 - 현재 스냅 앵글 이 스냅 각도를 2로 나눈 값보다 낮은 경우
        {
            return nowSnapAngle; //nowSnapAnlge 을 최종 각도로 리턴
        }
        else //만약 현재 각도 - 현재 스냅 앵글 이 스냅 각도를 2로 나눈 값보다 큰경우
        {
            return nowSnapAngle + snapAngle; //nowSnapAnlge에 앵글 각도를 더한 값(다음 스냅 앵글)을 리턴
        }

        
    }
}
