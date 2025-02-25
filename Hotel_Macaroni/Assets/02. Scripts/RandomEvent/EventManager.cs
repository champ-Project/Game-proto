using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using System.Collections;

//게임 내 이벤트 관리 스크립트
public class EventManager : MonoBehaviour
{
    //이벤트 분류및 정리를 위한 클래스
    [System.Serializable]
    public class EventSet
    {
        public bool isNowActive = false;
        public EventData eventData;
        [Header("Optional")]
        public GameObject prepareEventObject;
        public string notePageText;
    }
/*
    [System.Serializable]
    public class EventSummary
    {
        public int eventFloor;
        public string eventCode;
        public string eventHint;
    }
*/
    private GameManager gameManager;
    private PlayerController playerController;
    public List<EventSet> gameEvents = new List<EventSet>();
    [SerializeField] private TextAsset eventCSV;
    [SerializeField] private List<string[]> csvData = new List<string[]>();
    [SerializeField] private GameObject notePagePrefab;
    [SerializeField] private Volume volume;
    [SerializeField] public FilmGrain filmGrain;

    [SerializeField] private GameObject nowEventObject = null;

    public bool isDebuffOn = false;
    [SerializeField] private float debuffTime = 10f;
    private Coroutine nowCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        playerController = gameManager.playerController;
        volume.profile.TryGet<FilmGrain>(out filmGrain);
        ReadEventCSV();
    }

    //플레이어의 위치와 관계없이 발동하는 이벤트
    //플레이어가 있는 층 및 시간만 체크하면 됨
    public void GlobalEvent()
    {
        //단 추후 층별로 구별 필요
    }

    //플레이어 주변에서 발동하는 이벤트
    //아마 특정 이벤트를 특정 타이밍에 선택하고 트리거 활성화 하는 방향으로
    public void AroundEvent()
    {
        
    }

    public void NowActiveEvent(GameObject gameObject)
    {
        nowEventObject = gameObject;

        foreach(var eventSet in gameEvents)
        {
            if(eventSet.eventData.eventObject == nowEventObject)
            {
                eventSet.isNowActive = true;
            }
        }
    }

    public void EventFailed(string _eventCode)
    {
        Debug.Log("이벤트 실패");
        gameEvents.ForEach(_event => {
            if(_event.eventData.eventCode == _eventCode)
            {
                Debug.Log("이벤트 확인");
                string[] eventCSV = FindEventCSV(_event.eventData.eventCode);
                Debug.Log(eventCSV + "CSV라인내용");
                HintPageDrop(eventCSV);
                gameManager.PlayerDead("테스트");
            }
            else
            {
                Debug.Log("이벤트 종료 문제발생");
            }
        });
    }

    public void HintPageDrop(string[] _eventCSV)
    {
        RaycastHit hit;
        if(Physics.Raycast(playerController.transform.position, Vector3.down, out hit))
        {
            Debug.Log(hit.transform.gameObject.name + "레이충돌");
            if (hit.collider.CompareTag("Floor"))
            {
                Debug.Log("노트 소환");
                GameObject tornNoteObject = Instantiate(notePagePrefab, hit.point, Quaternion.identity);
                ItemDataSet _itemDataSet = tornNoteObject.GetComponent<ItemDataSet>();
                foreach(var _event  in gameEvents)
                {
                    if(_event.eventData.eventCode == _eventCSV[1])
                    {
                        _itemDataSet.thisEventData = _event.eventData;
                    }
                }
                _itemDataSet.eventCode = _eventCSV[1];
                _itemDataSet.eventFloor = playerController.nowFloor.ToString();
                Debug.Log(_itemDataSet.eventCode);
            }
        }
    }

    private void ReadEventCSV()
    {
        string[] lines = eventCSV.text.Split('\n');
        Regex csvPattern = new Regex("\\s*\"([^\"]*)\"\\s*|([^,]+)", RegexOptions.Compiled);

        foreach(string line in lines)
        {
            MatchCollection matches = csvPattern.Matches(line);
            List<string> row = new List<string>();

            foreach(Match match in matches)
            {
                if (match.Groups[1].Length > 0)
                {
                    row.Add(match.Groups[1].Value);
                }
                else if (match.Groups[2].Length > 0)
                {
                    row.Add(match.Groups[2].Value);
                }
            }
            if (row.Count > 0) csvData.Add(row.ToArray());
        }
    }

    public string[] FindEventCSV(string _eventCode)
    {
        if(csvData.Count > 1)
        {
            foreach(var eventCSV in csvData)
            {
                if (eventCSV[1] == _eventCode)
                {
                    Debug.Log(eventCSV.Length +"길이"+ string.Join(". ", eventCSV));
                    return eventCSV;
                }
            }
        }
        return null;
    }

    public string FindEventCSVPart(string _eventCode, string _targetText)
    {
        //Debug.Log("체크1");
        if(csvData.Count > 1)
        {
            for(int i = 0; i < csvData.Count; i++)
            {
                if (csvData[i][1] == _eventCode)
                {
                    int column = 0;
                    switch (_targetText)
                    {
                        case "kind": //이벤트 분류 리턴
                            column = 0;
                            break;
                        case "code": //이벤트 분류 리턴
                            column = 1;
                            break;
                        case "name": //이벤트 분류 리턴
                            column = 2;
                            break;
                        case "hint1": //이벤트 분류 리턴
                            column = 3;
                            break;
                        case "hint2": //이벤트 분류 리턴
                            column = 4;
                            break;
                            default:
                            break;
                    }
                    //Debug.Log("체크2");
                    return csvData[i][column];
                }
            }
        }
        Debug.Log("csv 텍스트값 찾지못함");
        return null ;
    }

    public void RandomDebuffOn()
    {
        if (isDebuffOn == true) return;

        int randomValue = Random.Range(0, 100);
        if (randomValue < 40)
        {
            Debug.Log("디버프발동1");

            nowCoroutine = StartCoroutine(PlayerSlowDebuff());
        }
        else if (randomValue < 80)
        {
            //StartCoroutine(PlayerEyeDebuff());
            Debug.Log("디버프발동2");
            nowCoroutine = StartCoroutine(PlayerSlowDebuff());
        }
        else
        {
            nowCoroutine = StartCoroutine(PlayerSlowDebuff());
            //DeadDebuff();
        }
    }

    private IEnumerator PlayerSlowDebuff()
    {
        isDebuffOn = true;
        GameManager.instance.playerController.PlayerMoveSpeed(1, 3);
        yield return new WaitForSeconds(debuffTime);
        GameManager.instance.playerController.PlayerMoveSpeed(3, 5);
        isDebuffOn = false;
        nowCoroutine = null;
        Debug.Log("디버프 코루틴종료");
    }

    /*private IEnumerator PlayerEyeDebuff()
    {

    }*/

    private void DeadDebuff()
    {

    }
}
