using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    private int currentPage = 0; // 대화문 개수 변수
    public bool IsStory = false;
    public bool IsDialogSet = false;
    private bool IsPreventDup = false;

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

        StartCoroutine("WaitAndSet");
    }

    public void CheckShowText(string text)
    {
        // 불러오기 했는데 -나 +가 포함되어 있으면 리턴
        if(GameManager.Instance.IsContinue && !IsPreventDup)
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

    IEnumerator WaitAndSet()
    {
        while (!IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
        showTextDup = talkDatas[0].showText[0];
        CheckShowText(showTextDup);
    }

}