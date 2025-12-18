using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        CheckRequire();

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

    private void CheckRequire()
    {
        for (int i = 0; i < 3; i++)
        {
            // 진실의 조각 관련 체크
            if (choiceStructs[i].select == "정원사를 알고 있다.")
            {
                if (TextManager.Instance.truthPiece < 1) choiceStructs[i].IsBlink = true;
                else
                {
                    int ran = UnityEngine.Random.Range(0, 10);
                    if (ran < 4) choiceStructs[i].IsBlink = false;
                    else choiceStructs[i].IsBlink = true;
                }
            }
            else if (choiceStructs[i].select == "그들이 익숙하다.")
            {
                // 진실의 조각 없으면 투명하게
                if (TextManager.Instance.truthPiece <= 0) choiceStructs[i].IsBlink = true;
                else choiceStructs[i].IsBlink = false;
            }

            // 챕터 4 사격: 확률에 의한 트리거 이동
            if (choiceStructs[i].select == "사격한다.")
            {
                if (choiceStructs[i].trigger == "4_2_A_1")
                {
                    int ran = UnityEngine.Random.Range(0, 10);
                    if (ran < 3) choiceStructs[i].trigger = "4_2_A_1";
                    else if (ran >= 3 && ran < 6) choiceStructs[i].trigger = "4_2_A_1_2";
                    else choiceStructs[i].trigger = "4_2_A_1_3";
                }
                else if (choiceStructs[i].trigger == "4_2_A_1_2")
                {
                    int ran = UnityEngine.Random.Range(0, 10);
                    if (ran < 5) choiceStructs[i].trigger = "4_2_A_1_2";
                    else choiceStructs[i].trigger = "4_2_A_1_3";
                }
            }

            // 챕터 4 상점 : 코인에 따른 선택지 상태변화
            if (choiceStructs[i].select == "물건을 더 둘러본다.")
            {
                if (Health.Instance.coin >= 1) choiceStructs[i].IsBlink = false;
                else choiceStructs[i].IsBlink = true;
            }

            if (choiceStructs[i].select == "주운 돈을 둔다.")
            {
                if (Health.Instance.coin >= 1) choiceStructs[i].IsBlink = false;
                else choiceStructs[i].IsBlink = true;
            }

            // 챕터 9 - 진실의 조각
            if (choiceStructs[i].select == "친구에 관한 이야기")
            {
                // 진실의 조각 3개 이상 없으면 투명하게
                if (TextManager.Instance.truthPiece < 3) choiceStructs[i].IsBlink = true;
                else choiceStructs[i].IsBlink = false;
            }
            else if (choiceStructs[i].select == "\" 좋아해야만 했어요. \"")
            {
                // 진실의 조각 1개 이상 없으면 투명하게
                if (TextManager.Instance.truthPiece < 1) choiceStructs[i].IsBlink = true;
                else choiceStructs[i].IsBlink = false;
            }

            // 챕터 10 - 진실의 조각
            if (choiceStructs[i].select == "\" 시아를 알고있어요. \"")
            {
                // 진실의 조각 1개 이상 없으면 투명하게
                if (TextManager.Instance.truthPiece < 1) choiceStructs[i].IsBlink = true;
                else choiceStructs[i].IsBlink = false;
            }
            else if (choiceStructs[i].select == "\" 알고 있어요. \"")
            {
                // 진실의 조각 1개 이상 없으면 투명하게
                if (TextManager.Instance.truthPiece < 1) choiceStructs[i].IsBlink = true;
                else choiceStructs[i].IsBlink = false;
            }

            //챕터8 : 탈출도구
            if (choiceStructs[i].select == "도구를 찾아본다." || choiceStructs[i].select == "도구를 더 찾는다.")
            {
                int ran = UnityEngine.Random.Range(0, 9);
                if (ran == 0) choiceStructs[i].trigger = "8_2";
                else choiceStructs[i].trigger = "8_2_" + ran;
            }
            else if (choiceStructs[i].select == "배를 띄워본다.")
            {
                if (TextManager.Instance.equips[0] && TextManager.Instance.equips[1] && TextManager.Instance.equips[2])
                {
                    choiceStructs[i].trigger = "8_2_b";
                }
                else
                {
                    choiceStructs[i].trigger = "8_2_a";
                }
            }
        }
    }
    private void CheckEquip()
    {

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
        // 랜덤 인카운터 - 전투
        if (text == "그들을 공격한다.")
        {
            int ran = UnityEngine.Random.Range(0, 10);
            if (ran < 5) TextManager.Instance.IsAttackFail = true;
        }
        else if (text == "그들을 밀친다.")
        {
            int ran = UnityEngine.Random.Range(0, 10);
            if (ran < 5) TextManager.Instance.IsAttackFail = true;
        }

        // 모자 장수의 슬픔 상담소
        if (text == "피가 묻은 종이")
        {
            TextManager.Instance.gambleItem = 0;
            SaveLoadManager.Instance.gambleItem = 0;
            SaveLoadManager.Instance.SaveGameData();
        }
        else if (text == "찢어진 종이")
        {
            TextManager.Instance.gambleItem = 1;
            SaveLoadManager.Instance.gambleItem = 1;
            SaveLoadManager.Instance.SaveGameData();
        }
        else if (text == "투명한 종이")
        {
            TextManager.Instance.gambleItem = 2;
            SaveLoadManager.Instance.gambleItem = 2;
            SaveLoadManager.Instance.SaveGameData();
        }

        if (text.Contains("-"))
        {
            //숫자만 자르기
            int count = int.Parse(Regex.Match(text, @"\d+").Value);

            if (text.Contains("체력")) Health.Instance.HealthMinus(count);
            else if (text.Contains("정신력")) Health.Instance.MentalMinus(count);
            else if (text.Contains("돈")) Health.Instance.CoinMinus(count);
        }
        else if (text.Contains("+"))
        {
            //숫자만 자르기
            int count = int.Parse(Regex.Match(text, @"\d+").Value);

            if (text.Contains("체력")) Health.Instance.HealthPlus(count);
            else if (text.Contains("정신력")) Health.Instance.MentalPlus(count);
            else if (text.Contains("돈")) Health.Instance.CoinPlus(count);
        }
    }
}
