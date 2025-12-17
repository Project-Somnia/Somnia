using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class TextManager : MonoBehaviour
{
    public GameObject ChapterObject;
    public TextMeshProUGUI storyText;
    string[] dialogStrings;
    TalkData[] talkDatas;
    public string storyEventName;

    public string[] selectText = new string[3];
    public string[] triggerEvent = new string[3];
    public string showTextDup;

    private int currentPage = 0; // 대화문 개수 변수
    public string pendingMainEventId;
    public bool IsStory = false;
    public bool IsDialogSet = false;
    public bool IsFastText = false;
    private bool IsPreventDup = false;
    private bool IsPreventFadeDup = false;
    private bool IsPaintEmpty = false;

    private float fadeTime = 2f;

    [Header("Paint Sets")]
    public Image paint;
    public Image newPaint;
    public Sprite[] paintSprites = new Sprite[10];
    public Sprite empty;
    private string currentPaint = "";
    private string paintNum = "";
    private string curSplitEvent = "";
    private int paintIdx = 0;
    private int fadeCnt = 0;

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
    }

    private void Update()
    {
        if (IsStory)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) IsFastText = true;
            TypingManager.Instance.GetInputDown();
            CheckShowText(talkDatas[0].showText[currentPage]);
            if (TypingManager.Instance.isTypingEnd)
            {
                if (currentPage == talkDatas.Length && TypingManager.Instance.isDialogEnd)
                {
                    IsStory = false;
                    IsFastText = false;
                    StoryChoice.fadeChoice();
                    currentPage = 0;
                    return;
                }
            }
        }
    }

    public void SetDialogue(string eventNumber)
    {
        storyText.text = "";
        storyEventName = eventNumber;
        CheckAudioStat();

        // 텍스트 정보 가져오기
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TypingManager.Instance.Typing(talkDatas[0].showText, storyText);
        currentPage++;

        // 선택지 세팅
        selectText[0] = talkDatas[0].selectText1;
        selectText[1] = talkDatas[0].selectText2;
        selectText[2] = talkDatas[0].selectText3;

        triggerEvent[0] = talkDatas[0].triggerEvent1;
        triggerEvent[1] = talkDatas[0].triggerEvent2;
        triggerEvent[2] = talkDatas[0].triggerEvent3;

        StoryChoice.setChoiceText();
        IsStory = true;

        // 사진 세팅
        SetPaint(talkDatas[0].eventImage);

        SaveLoadManager.Instance.eventNumber = eventNumber;
        SaveLoadManager.Instance.SaveGameData();

        // 스토리 텍스트 확인
        showTextDup = talkDatas[0].showText[0];
        //CheckShowText(showTextDup);

        //StartCoroutine("WaitAndSet");
    }

    public void SetDialogueFromChoice(string triggerEventId)
    {
        if (triggerEventId == "RETURN_MAIN")
        {
            if (!string.IsNullOrEmpty(pendingMainEventId))
            {
                string mainId = pendingMainEventId;

                // 한 번 쓰면 비워준다 (다음 랜덤에 영향 없게)
                pendingMainEventId = null;

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
        if (EncounterFlowManager.Instance != null)
        {
            // storyEventName = 현재 진행 중인 메인/랜덤 이벤트
            finalId = EncounterFlowManager.Instance.DecideNextEvent(storyEventName, triggerEventId);
        }
        SetDialogue(finalId);
    }

    public void CheckShowText(string text)
    {
        // 불러오기 했는데 -나 +가 포함되어 있으면 리턴
        if (GameManager.Instance.IsContinue && !IsPreventDup)
        {
            IsPreventDup = true;
            return;
        }
        if (text.Contains("-1"))
        {
            if (text.Contains("진실의 조각") && GameManager.Instance.truthPiece > 0)
            {
                GameManager.Instance.truthPiece -= 1;
                SaveLoadManager.Instance.truthPiece = GameManager.Instance.truthPiece;
                SaveLoadManager.Instance.SaveGameData();
            }
            else if (text.Contains("체력")) Health.healthM();
            else if (text.Contains("정신력")) Health.mentalM();
            else if (text.Contains("돈")) Health.coinM();
        }
        else if (text.Contains("+1"))
        {
            if (text.Contains("진실의 조각") && GameManager.Instance.truthPiece < 5)
            {
                GameManager.Instance.truthPiece += 1;
                SaveLoadManager.Instance.truthPiece = GameManager.Instance.truthPiece;
                SaveLoadManager.Instance.SaveGameData();
            }
            else if (text.Contains("체력")) Health.healthP();
            else if (text.Contains("정신력")) Health.mentalP();
            else if (text.Contains("돈")) Health.coinP();
        }
    }
    private void SetPaint(string paintName)
    {
        paintName = paintName.Replace(".png", "");
        paintNum = paintName.Split("_")[0];

        if (paintName == "Empty" || paintName == "") IsPaintEmpty = true;

        if (currentPaint == "" || currentPaint == "empty" || currentPaint != paintName)
        {
            currentPaint = paintName;
            SaveLoadManager.Instance.paintName = paintName;
            SaveLoadManager.Instance.SaveGameData();

            paintIdx = Array.FindIndex(paintSprites, x => currentPaint.Contains(x.name));

            FadePaint(paint, newPaint);
        }
    }

    void FadePaint(Image curPaint, Image nextPaint)
    {
        curPaint.gameObject.SetActive(true);
        nextPaint.gameObject.SetActive(true);
        // 짝수
        if (fadeCnt % 2 == 0)
        {
            if (IsPaintEmpty)
            {
                IsPaintEmpty = false;
                nextPaint.sprite = empty;
            }
            else nextPaint.sprite = paintSprites[paintIdx];

            if (IsPreventFadeDup)
            {
                Color curCol = curPaint.color;
                curCol.a = 1f;
                curPaint.color = curCol;
            }

            Color nextCol = nextPaint.color;
            nextCol.a = 0f;
            nextPaint.color = nextCol;

            curPaint.DOFade(0f, fadeTime);
            nextPaint.DOFade(1f, fadeTime);
        }
        else
        {
            if (IsPaintEmpty)
            {
                IsPaintEmpty = false;
                curPaint.sprite = empty;
            }
            else curPaint.sprite = paintSprites[paintIdx];

            Color curCol = curPaint.color;
            curCol.a = 0f;
            curPaint.color = curCol;

            Color nextCol = nextPaint.color;
            nextCol.a = 1f;
            nextPaint.color = nextCol;

            curPaint.DOFade(1f, fadeTime);
            nextPaint.DOFade(0f, fadeTime);
        }
        IsPreventFadeDup = true;
        fadeCnt++;
    }

    IEnumerator WaitAndSet()
    {
        while (!IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (GameManager.Instance.IsContinue)
        {
            currentPaint = SaveLoadManager.Instance.paintName;
            if (currentPaint == "empty") paint.sprite = empty;
            else
            {
                paintIdx = Array.FindIndex(paintSprites, x => currentPaint.Contains(x.name));
                paint.sprite = paintSprites[paintIdx];
            }
        }
        paint.DOFade(1f, fadeTime);
    }

    void CheckAudioStat()
    {
        // 오디오 트랙 설정
        string splitEventName = storyEventName.Split('_')[0];
        // 똑같으면 return
        if (curSplitEvent == splitEventName) return;

        //1,2,3은 같은 음악
        if (curSplitEvent == "1" && splitEventName == "2" || curSplitEvent == "3")
            return;

        if (curSplitEvent == "2" && splitEventName == "1" || curSplitEvent == "3")
            return;

        if (curSplitEvent == "3" && splitEventName == "1" || curSplitEvent == "2")
            return;

        //4,5,6은 같은 음악
        if (curSplitEvent == "4" && splitEventName == "5" || curSplitEvent == "6")
            return;

        if (curSplitEvent == "5" && splitEventName == "4" || curSplitEvent == "6")
            return;

        if (curSplitEvent == "6" && splitEventName == "4" || curSplitEvent == "5")
            return;

        //7,8,9는 같은 음악
        if (curSplitEvent == "7" && splitEventName == "8" || curSplitEvent == "9")
            return;

        if (curSplitEvent == "8" && splitEventName == "7" || curSplitEvent == "9")
            return;

        if (curSplitEvent == "9" && splitEventName == "7" || curSplitEvent == "8")
            return;

        //10,11은 같은 음악
        if (curSplitEvent == "10" && splitEventName == "11")
            return;

        Debug.Log("speventname = " + splitEventName);
        curSplitEvent = splitEventName;
        AudioManager.Instance.SetAudioTrack(splitEventName);
    }

}