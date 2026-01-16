using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TextManager : MonoBehaviour
{
    public GameObject ChapterObject;
    public GameObject commingsoon;
    public GameObject textLogBox;
    public Transform contentTr;
    public GameOverUI gameOverUI;
    public TextMeshProUGUI storyText;
    public ScrollRect scrollRect;

    string[] dialogStrings;
    public TalkData[] talkDatas;
    public string storyEventName;


    public string[] selectText = new string[3];
    public string[] triggerEvent = new string[3];
    public string showTextDup;

    public int currentPage = 0; // 대화문 개수 변수
    public string pendingMainEventId;
    public bool IsStory = false;
    public bool IsDialogSet = false;
    public int charSpeedLevel = 0;
    public string originNextEvent = "";
    public string curSplitEvent = "1";
    private float scrollSensitivity = 10f;



    private static TextManager instance;
    public static TextManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No TextManagerInstance");
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        StartCoroutine("WaitAndSet");
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = scrollSensitivity;
    }

    private void Update()
    {
        if (IsStory)
        {
            TypingManager.Instance.GetInputDown();
            if (TypingManager.Instance.isTypingEnd)
            {
                if (currentPage == talkDatas.Length && TypingManager.Instance.isDialogEnd)
                {
                    TypingManager.Instance.IsSkipDialog = false;
                    IsStory = false;
                    //TypingManager.Instance.IsSkipDialog = false;
                    if (!GameManager.Instance.IsGameOver)
                    {
                        StoryChoice.fadeChoice();
                        currentPage = 0;
                        return;
                    }
                    if(GameManager.Instance.IsGameOver && !GameManager.Instance.IsMentalMor)
                    {
                        gameOverUI.ShowHpGameOver();
                    }
                    else if(GameManager.Instance.IsGameOver && GameManager.Instance.IsMentalMor)
                    {
                        gameOverUI.ShowMentalGameOver();
                    }
                    //게임오버상태면 선택지 표시하지 않음
                }
            }
        }
    }

    public void SetDialogue(string eventNumber)
    {
        storyText.text = "";
        storyEventName = eventNumber;

        // 텍스트 정보 가져오기
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TriggerManager.Instance.CheckShowText(talkDatas[0].showText);
        // 한번 더 체크
        if (GameManager.Instance.IsZero)
        {
            if (Health.Instance.health == 0 && !GameManager.Instance.IsGameOver) 
            {
                GameManager.Instance.IsGameOver = true;
                SetDialogue("D_1");
                return;
            }

            else if(Health.Instance.mental == 0 && !GameManager.Instance.IsMentalMor)
            {
                GameManager.Instance.IsMentalMor = true;
                SetDialogue("T_1");
                return;
            }
            else if(storyEventName == "T_4") GameManager.Instance.IsGameOver = true;
        }

        //오디오 세팅
        AudioManager.Instance.CheckAudioStat(storyEventName.Split('_')[0]);
        // 사진 세팅
        PaintManager.Instance.SetPaint(talkDatas[0].eventImage);
        TypingManager.Instance.Typing(talkDatas[0].showText, storyText);
        currentPage++;
        
        //로그에 기록
        GameObject newLog = Instantiate(textLogBox,contentTr);
        TextMeshProUGUI textLog = newLog.GetComponentInChildren<TextMeshProUGUI>();
        for(int i=0;i<talkDatas[0].showText.Length;i++) textLog.text += talkDatas[0].showText[i];
        scrollRect.verticalNormalizedPosition = 0f;

        // 선택지 세팅
        selectText[0] = talkDatas[0].selectText1;
        selectText[1] = talkDatas[0].selectText2;
        selectText[2] = talkDatas[0].selectText3;

        triggerEvent[0] = talkDatas[0].triggerEvent1;
        triggerEvent[1] = talkDatas[0].triggerEvent2;
        triggerEvent[2] = talkDatas[0].triggerEvent3;

        StoryChoice.setChoiceText();
        IsStory = true;

        SaveLoadManager.Instance.eventNumber = eventNumber;
        SaveLoadManager.Instance.SaveGameData();

        // 스토리 텍스트 확인
        showTextDup = talkDatas[0].showText[0];
    }

    public void SetDialogueFromChoice(string triggerEventId)
    {
        if (triggerEventId == "RETURN_MAIN")
        {
            if(GameManager.Instance.IsMentalMor)
            {
                GameManager.Instance.IsZero = false;
                GameManager.Instance.IsMentalMor = false;
                GameManager.Instance.IsRebirth = true;

                pendingMainEventId = originNextEvent;
                curSplitEvent = storyEventName.Split("_")[0];
                SetDialogue(originNextEvent);
                return;
            }
            else if (!string.IsNullOrEmpty(pendingMainEventId))
            {
                string mainId = pendingMainEventId;

                // 한 번 쓰면 비워준다 (다음 랜덤에 영향 없게)
                pendingMainEventId = null;
                curSplitEvent = storyEventName.Split("_")[0];
                SetDialogue(mainId);
                return;
            }
            else
            {
                Debug.LogError("RETURN_MAIN인데 pendingMainEventId가 비어 있음");
                return;
            }
        }
        string finalId = triggerEventId;

        // EncounterFlowManager가 존재하면 랜덤 인카운터 로직 적용
        if (EncounterFlowManager.Instance != null && !GameManager.Instance.IsZero)
        {
            // storyEventName = 현재 진행 중인 메인/랜덤 이벤트
            finalId = EncounterFlowManager.Instance.DecideNextEvent(storyEventName, triggerEventId);
        }
        SetDialogue(finalId);
    }

    IEnumerator WaitAndSet()
    {
        while (!IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

}