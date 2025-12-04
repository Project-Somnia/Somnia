using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StoryChoice : MonoBehaviour
{
    public float fadeTime = 1f;

    [Header("Choice Button and Text")]
    public Image choice1;
    public TextMeshProUGUI choiceText1;

    public Image choice2;
    public TextMeshProUGUI choiceText2;

    public Image choice3;
    public TextMeshProUGUI choiceText3;

    public GameObject underline;
    public Sprite canSelectChoice;
    public Sprite cantSelectChoice;

    public static Action fadeChoice;
    public static Action setChoiceText;

    private bool IsCanSelect = false;
    private struct ChoiceStruct
    {
        public string select;
        public string trigger;
        public bool IsBlink;

        public ChoiceStruct(string select, string trigger, bool IsBlink)
        {
            this.select = select;
            this.trigger = trigger;
            this.IsBlink = IsBlink;
        }
    }
    ChoiceStruct[] choiceStructs = new ChoiceStruct[3];
    List<int> values = new List<int> { 0, 1, 2 };

    void Awake()
    {
        fadeChoice = () =>
        {
            StartCoroutine("FadeIn");
        };

        setChoiceText = () =>
        {
            SetChoiceText();
        };
    }


    IEnumerator FadeIn()
    {
        FadeInChoice(choice1, choiceText1);
        yield return new WaitForSeconds(0.5f);

        FadeInChoice(choice2, choiceText2);
        yield return new WaitForSeconds(0.5f);

        FadeInChoice(choice3, choiceText3);
        yield return new WaitForSeconds(0.5f);
        IsCanSelect = true;
    }

    void FadeInChoice(Image fadeImg, TextMeshProUGUI fadeText)
    {
        fadeImg.gameObject.SetActive(true);
        fadeText.gameObject.SetActive(true);

        Color color = fadeImg.color;
        color.a = 0f;
        fadeImg.color = color;

        Color textColor = fadeText.color;
        textColor.a = 0f;
        fadeText.color = textColor;

        fadeImg.DOFade(1f, fadeTime);
        fadeText.DOFade(1f, fadeTime);
    }

    public void SetChoiceText()
    {
        choiceStructs[0].IsBlink = false;
        choiceStructs[1].IsBlink = false;
        choiceStructs[2].IsBlink = false;

        int chkGap = 2;
        int notGap = 0;
        // 랜덤하게 섞기
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(i + "현재텍스트" + TextManager.Instance.selectText[i]);
            Debug.Log(i + "옮긴텍스트" + choiceStructs[chkGap].select);
            if (TextManager.Instance.selectText[i] == "")
            {
                choiceStructs[chkGap].select = TextManager.Instance.selectText[i];
                choiceStructs[chkGap].trigger = TextManager.Instance.triggerEvent[i];
                choiceStructs[chkGap].IsBlink = true;
                chkGap -= 1;
            }
            else
            {
                choiceStructs[notGap].select = TextManager.Instance.selectText[i];
                choiceStructs[notGap].trigger = TextManager.Instance.triggerEvent[i];
                notGap++;
            }
        }
        // for (int i = 0; i <= chkGap; i++)
        // {
        //     choiceStructs[i].select = TextManager.Instance.selectText[i];
        //     choiceStructs[i].trigger = TextManager.Instance.triggerEvent[i];
        // }

        if (choiceStructs[0].IsBlink) choice1.sprite = cantSelectChoice;
        else choice1.sprite = canSelectChoice;
        if (choiceStructs[1].IsBlink) choice2.sprite = cantSelectChoice;
        else choice2.sprite = canSelectChoice;
        if (choiceStructs[2].IsBlink) choice3.sprite = cantSelectChoice;
        else choice3.sprite = canSelectChoice;

        choiceText1.text = choiceStructs[0].select;
        choiceText2.text = choiceStructs[1].select;
        choiceText3.text = choiceStructs[2].select;
    }

    public void ChoiceOff()
    {
        choice1.gameObject.SetActive(false);
        choiceText1.gameObject.SetActive(false);

        choice2.gameObject.SetActive(false);
        choiceText2.gameObject.SetActive(false);

        choice3.gameObject.SetActive(false);
        choiceText3.gameObject.SetActive(false);

        Underline.reDraw();
    }
    public void Choice1()
    {
        if (IsCanSelect && !choiceStructs[0].IsBlink)
        {
            IsCanSelect = false;
            CheckChoice(choiceText1.text);
            ChoiceOff();
            TextManager.Instance.SetDialogueFromChoice(choiceStructs[0].trigger);      
        }
    }
    public void Choice2()
    {
        if (IsCanSelect && !choiceStructs[1].IsBlink)
        {
            IsCanSelect = false;
            CheckChoice(choiceText2.text);
            ChoiceOff();
            TextManager.Instance.SetDialogueFromChoice(choiceStructs[1].trigger);  
        }
    }
    public void Choice3()
    {
        if (IsCanSelect && !choiceStructs[2].IsBlink)
        {
            IsCanSelect = false;
            CheckChoice(choiceText3.text);
            ChoiceOff();
            TextManager.Instance.SetDialogueFromChoice(choiceStructs[2].trigger);
        }
    }

    private void CheckChoice(string text)
    {
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
}
