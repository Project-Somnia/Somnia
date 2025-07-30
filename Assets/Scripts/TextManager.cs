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

    [Header("StoryUI")]
    public GameObject talkPanel;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI nameText;
    public Animator storyImageAnimator;
    string[] dialogStrings;
    TalkData[] talkDatas;
    public string storyEventName;
    private int currentPage = 0; // 대화문 개수 변수
    public bool IsStory = true;
    public bool IsSkipStory;
    public bool IsBlockTextUpdate;

    private static TextManager _instance;
    public static TextManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType(typeof(TextManager)) as TextManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Start()
    {
        storyEventName = "0_0";
        SetDialogue();
    }
    public void SetDialogue()
    {
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TypingManager._instance.Typing(talkDatas[0].contexts, storyText);
        //SetNameColor(nameText.text);
        currentPage++;
    }

    public void SetNameColor(string name)
    {
        if (name.Trim() == "데커스")
        {
            nameText.color = new Color32(38, 255, 175, 255);
        }
        else if (name.Trim() == "오프시아")
        {
            nameText.color = new Color32(254, 86, 39, 255);
        }
    }

    // State Pattern으로 변경하기.
    // 코루틴으로 바꾸는것이 좋아 보인다.

    private void Update()
    {
        if (IsBlockTextUpdate) { return; }

        // if (TypingManager._instance.isTypingEnd)
        // {
        //     SetTextCursor();
        // }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TypingManager._instance.GetInputDown();
            if (TypingManager._instance.isTypingEnd)
            {
                if (currentPage == talkDatas.Length && TypingManager._instance.isDialogEnd)
                {
                    currentPage = talkDatas.Length;
                    IsStory = false;
                    nameText.text = "";
                    currentPage = 0;
                    IsBlockTextUpdate = true;
                }
                TypingManager._instance.Typing(talkDatas[currentPage].contexts, storyText);
                currentPage++;
            }
        }
    }

}