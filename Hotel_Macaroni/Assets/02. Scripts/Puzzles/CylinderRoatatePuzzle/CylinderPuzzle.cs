using UnityEngine;

public class CylinderPuzzle : MonoBehaviour
{
    public Transform[] cylinderParts;
    public float targetYRotation = 0f;
    [SerializeField] private bool isClear = false;

    public void CheckPuzzle()
    {
        for (int i = 0; i < cylinderParts.Length; i++)
        {
            float currentRotation = NormalizeAngle(cylinderParts[i].localEulerAngles.y);

            // 각 실린더의 회전이 초기 회전과 비슷한지 확인
            if (Mathf.Abs(currentRotation - targetYRotation) > 1f)
            {
                Debug.Log("퍼즐이 맞춰지지 않았습니다.");
                return;
            }
        }
        Debug.Log("퍼즐 완성!");

        CylinderPuzzleClear();
    }

    private float NormalizeAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }

    private void CylinderPuzzleClear()
    {
        //퍼즐 클리어시 아이템 획득 및
    }
}
