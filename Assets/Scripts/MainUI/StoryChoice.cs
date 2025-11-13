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

    public static Action fadeChoice;
    public static Action setChoiceText;

    private bool IsCanSelect = false;
    private string[] textArr = new string[3];
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

        textArr[values[0]] = TextManager.Instance.choiceText1;
        textArr[values[1]] = TextManager.Instance.choiceText2;
        textArr[values[2]] = TextManager.Instance.choiceText3;

        choiceText1.text = textArr[0];
        choiceText2.text = textArr[1];
        choiceText3.text = textArr[2];
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
            Health.healthM();
        }
    }
    public void Choice2()
    {
        if (IsCanSelect)
        {
            Health.mentalM();
        }
    }
    public void Choice3()
    {
        if (IsCanSelect)
        {
            Health.coinM();
        }
    }
}
