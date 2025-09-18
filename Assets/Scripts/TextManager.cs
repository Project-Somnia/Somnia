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

    public GameObject talkPanel;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI nameText;
    public Animator storyImageAnimator;
    string[] dialogStrings;
    TalkData[] talkDatas;
    public string storyEventName;
    private int currentPage = 0; // 대화문 개수 변수
    public bool IsStory = false;

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
    public void SetDialogue()
    {
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TypingManager._instance.Typing(talkDatas[0].showText, storyText);
        currentPage++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !IsStory)
        {
            IsStory = true;
            storyEventName = "1_0";
            SetDialogue();
        }
        if (Input.GetKeyDown(KeyCode.S) && !IsStory)
        {
            IsStory = true;
            storyEventName = "1_1";
            SetDialogue();
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) && IsStory)
        {
            TypingManager._instance.GetInputDown();
            if (TypingManager._instance.isTypingEnd)
            {
                if (currentPage == talkDatas.Length && TypingManager._instance.isDialogEnd)
                {
                    currentPage = talkDatas.Length;
                    IsStory = false;
                    currentPage = 0;
                    //storyText.text = "";
                    Debug.Log("대사 끝");
                    return;
                }
                TypingManager._instance.Typing(talkDatas[currentPage].showText, storyText);
                currentPage++;
            }
        }
    }

}