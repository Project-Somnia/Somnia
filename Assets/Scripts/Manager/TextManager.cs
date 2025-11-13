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
    public TextMeshProUGUI storyText;
    string[] dialogStrings;
    TalkData[] talkDatas;
    public string storyEventName;

    public string choiceText1;
    public string choiceText2;
    public string choiceText3;

    private int currentPage = 0; // 대화문 개수 변수
    public bool IsStory = false;
    public bool IsDialogSet = false;

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
                    Debug.Log("대사 끝");
                    return;
                }
                TypingManager.Instance.Typing(talkDatas[currentPage].showText, storyText);
                currentPage++;
            }
        }
    }

    public void SetDialogue(string eventNumber)
    {
        storyEventName = eventNumber;
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TypingManager.Instance.Typing(talkDatas[0].showText, storyText);
        currentPage++;

        choiceText1 = talkDatas[0].selectText1;
        choiceText2 = talkDatas[0].selectText2;
        choiceText3 = talkDatas[0].selectText3;

        StoryChoice.setChoiceText();
        IsStory = true;
    }

    

}