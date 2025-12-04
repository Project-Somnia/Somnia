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
    public GameObject RandomObject;
    public TextMeshProUGUI storyText;
    string[] dialogStrings;
    TalkData[] talkDatas;
    public string storyEventName;

    public string[] selectText = new string[3];
    public string[] triggerEvent = new string[3];
    public string showTextDup;

    public bool IsStory = false;
    public bool IsDialogSet = false;
    private bool IsPreventDup = false;
    private bool IsPreventFadeDup = false;

    private int currentPage = 0; // 대화문 개수 변수
    private float fadeTime = 2f;

    [Header("Paint Sets")]
    public Image paint;
    public Image newPaint;
    public Sprite[] paintSprites = new Sprite[9];
    private string currentPaint = "";
    private string paintNum = "";
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
        paint.DOFade(1f,fadeTime);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && IsStory)
        {
            TypingManager.Instance.GetInputDown();
            if (TypingManager.Instance.isTypingEnd)
            {
                if (currentPage == talkDatas.Length && TypingManager.Instance.isDialogEnd)
                {

                    currentPage = talkDatas.Length;
                    IsStory = false;
                    StoryChoice.fadeChoice();
                    currentPage = 0;
                    //storyText.text = "";
                    return;

                }
                TypingManager.Instance.Typing(talkDatas[currentPage].showText, storyText);
                currentPage++;
            }
        }
    }

    public void SetDialogue(string eventNumber)
    {
        storyText.text = "";
        storyEventName = eventNumber;

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
        CheckShowText(showTextDup);

        //StartCoroutine("WaitAndSet");
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
            if (text.Contains("체력")) Health.healthM();
            else if (text.Contains("정신력")) Health.mentalM();
            else if (text.Contains("돈")) Health.coinM();
        }
        else if (text.Contains("+1"))
        {
            if (text.Contains("체력")) Health.healthP();
            else if (text.Contains("정신력")) Health.mentalP();
            else if (text.Contains("돈")) Health.coinP();
        }
    }
    private void SetPaint(string paintName)
    {
        paintName = paintName.Replace(".png", "");
        if(paintName == "Empty") return;
        paintNum = paintName.Split("_")[0];
        
        if (currentPaint == "" || currentPaint != paintName)
        {
            currentPaint = paintName;
            paintIdx = Array.FindIndex(paintSprites, x => currentPaint.Contains(x.name));
            if(IsPreventFadeDup) FadePaint(paint, newPaint);
        }
        IsPreventFadeDup = true;
    }

    void FadePaint(Image curPaint, Image nextPaint)
    {
        curPaint.gameObject.SetActive(true);
        nextPaint.gameObject.SetActive(true);
        // 짝수
        if (fadeCnt % 2 == 0)
        {
            nextPaint.sprite = paintSprites[paintIdx];

            Color curCol = curPaint.color;
            curCol.a = 1f;
            curPaint.color = curCol;

            Color nextCol = nextPaint.color;
            nextCol.a = 0f;
            nextPaint.color = nextCol;

            curPaint.DOFade(0f, fadeTime);
            nextPaint.DOFade(1f, fadeTime);
        }
        else
        {
            curPaint.sprite = paintSprites[paintIdx];

            Color curCol = curPaint.color;
            curCol.a = 0f;
            curPaint.color = curCol;

            Color nextCol = nextPaint.color;
            nextCol.a = 1f;
            nextPaint.color = nextCol;

            curPaint.DOFade(1f, fadeTime);
            nextPaint.DOFade(0f, fadeTime);
        }
        fadeCnt++;
    }

    // IEnumerator WaitAndSet()
    // {
    //     while (!IsDialogSet)
    //     {
    //         yield return new WaitForSeconds(0.1f);
    //     }
    //     showTextDup = talkDatas[0].showText[0];
    //     CheckShowText(showTextDup);
    // }

}