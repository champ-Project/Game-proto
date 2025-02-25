using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GuideNoteManager : MonoBehaviour
{
    [SerializeField] private GameObject noteUI;
    [SerializeField] private GameObject[] notePages;
    public Button nextBtn;
    public Button prevBtn;
    [SerializeField] private List<string> tornNotes = new List<string>();

    [SerializeField] private GameObject nameInputUI;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Text nameText;

    public GameObject historyPagePrefab;
    [SerializeField] private GameObject[] tornNotePages;
    [SerializeField] private GameObject tornNotePrefab;
    [SerializeField] private Transform tornNotesTrans;
    public TextAsset historyCSV;
    private List<string[]> csvData = new List<string[]>();

    public GameManager gameManager;
    private EventManager eventManager;

    [System.Serializable]
    public class Page
    {
        public List<string> textLine = new List<string>();
    }

    [SerializeField] private List <Page> pages = new List<Page>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.instance;
        eventManager = gameManager.eventManager;
        //saveBtn.onClick.AddListener(SaveName);

        saveBtn.interactable = false;

        //nameInputField.onValueChanged.AddListener(OnInputValueChanged);

        ReadHistoryCSV();
    }

    //현재 미사용
    void OnInputValueChanged(string input)
    {
        // 입력된 텍스트가 1글자 이상일 때 버튼 활성화
        saveBtn.interactable = !string.IsNullOrEmpty(input);
    }

    //현재 미사용
    public void NoteSet()
    {
        noteUI.SetActive(true);
        nameInputField.text = "";
        nameInputUI.SetActive(true);
        gameManager.isGetNote = true;
        gameManager.playerController.PlayerDontMove(true);
        gameManager.playerController.CursorState(true);
    }

    //현재 미사용
    public void SaveName()
    {
        if(nameInputField.text != null)
        {
            nameText.text = nameInputField.text;
            GameManager.instance.nowPlayerName = nameText.text;
            nameInputUI.SetActive(false);
        }
    }

    public void AddHintNote(ItemDataSet _ItemDataSet)
    {

        if(_ItemDataSet != null)
        {
            Debug.Log("이벤트데이타 확인");
            Debug.Log(_ItemDataSet.eventCode);
            Debug.Log(_ItemDataSet.gameObject.name) ;
            string eventHint = gameManager.eventManager.FindEventCSVPart(_ItemDataSet.eventCode, "hint1");
            string eventHintLocation = _ItemDataSet.eventFloor;
            //tornNotes.Add(eventHint);
            GameObject tornNotePage = Instantiate(tornNotePrefab, tornNotesTrans);
            TornNote tornNote = tornNotePage.GetComponent<TornNote>();

            tornNote.hintText.text = eventHint;
            tornNote.pickupFloorText.text = "획득 장소 :" + eventHintLocation + "층";
        }
        
    }

    /*public void AddUsersHistory(int _historyLevel, int _count)
    {
        foreach(var nowCSV in csvData)
        {
            if(nowCSV.)
        }
    }*/

    private void ReadHistoryCSV()
    {
        if (historyCSV == null) return;

        string[] lines = historyCSV.text.Split('\n');
        Regex csvPattern = new Regex("\\s*\"([^\"]*)\"\\s*|([^,]+)", RegexOptions.Compiled);

        foreach (string line in lines)
        {
            MatchCollection matches = csvPattern.Matches(line);
            List<string> row = new List<string>();

            foreach (Match match in matches)
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
}
