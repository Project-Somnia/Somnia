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

    public static Action fadeChoice;
    public static Action setChoiceText;

    private bool IsCanSelect = false;
    private struct ChoiceStruct
    {
        public string select;
        public string trigger;

        public ChoiceStruct(string select, string trigger)
        {
            this.select = select;
            this.trigger = trigger;
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
        RandomShuffle();
        choiceStructs[0].select = TextManager.Instance.selectText[values[0]];
        choiceStructs[0].trigger = TextManager.Instance.triggerEvent[values[0]];

        choiceStructs[1].select = TextManager.Instance.selectText[values[1]];
        choiceStructs[1].trigger = TextManager.Instance.triggerEvent[values[1]];

        choiceStructs[2].select = TextManager.Instance.selectText[values[2]];
        choiceStructs[2].trigger = TextManager.Instance.triggerEvent[values[2]];

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

    private void RandomShuffle()
    {
        // 랜덤하게 섞기
        for (int i = 0; i < values.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, values.Count);
            int temp = values[i];
            values[i] = values[randomIndex];
            values[randomIndex] = temp;
        }
    }
    public void Choice1()
    {
        if (IsCanSelect)
        {
            IsCanSelect = false;
            CheckChoice(choiceText1.text);
            ChoiceOff();
            TextManager.Instance.SetDialogueFromChoice(choiceStructs[0].trigger);      
        }
    }
    public void Choice2()
    {
        if (IsCanSelect)
        {
            IsCanSelect = false;
            CheckChoice(choiceText2.text);
            ChoiceOff();
            TextManager.Instance.SetDialogueFromChoice(choiceStructs[1].trigger);  
        }
    }
    public void Choice3()
    {
        if (IsCanSelect)
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
