using UnityEngine;

public enum LightKind
{
    Hallway,
    Room
}

public class LightManager : MonoBehaviour
{
    //public GameObject[] lightObjects;
    public LightSwitch[] lightSwitchs;


    //복도,방 분류에 따른 해당 조명만 조절
    public void LightsStateChangeByType(LightKind _lightKind, bool state)
    {
        foreach (var light in lightSwitchs)
        {
            if(light.lightKind == _lightKind)
            {
                light.LightStateChange(state);
            }
        }
    }

    //밤에 꺼지는 조명들 제어
    public void LightUpOutAtNight(bool state)
    {
        foreach (var light in lightSwitchs)
        {
            //저녁에 꺼지게 설정된 라이트
            if (light.isNightOut)
            {
                light.LightStateChange(state);  //불끄기 or 불켜기
            }
        }
    }
}
