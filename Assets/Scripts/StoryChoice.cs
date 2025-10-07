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


    void Awake()
    {
        fadeChoice = () =>
        {
            StartCoroutine("FadeIn");
        };
    }

    IEnumerator FadeIn()
    {
        FadeInChoice(choice1, choiceText1);
        yield return new WaitForSeconds(1.0f);

        FadeInChoice(choice2, choiceText2);
        yield return new WaitForSeconds(1.0f);

        FadeInChoice(choice3, choiceText3);
        yield return new WaitForSeconds(1.0f);
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
}
