using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using UnityEngine.Audio;

public class TriggerManager : MonoBehaviour
{
    
    [Header("세이브로드 필요한 아이템들")]
    public bool[] equips = new bool[3]; // 각 순서별로 통나무, 노끈, 노
    public int truthPiece = 0;
    public int gambleItem = 0;
    [Header("기타")]
    public bool IsAttackFail = false;
    private bool IsPreventDup = false;
    [Header("Audio Sets")]
    public AudioSource audioSource;
    public AudioClip plus;
    public AudioClip minus;

    private string ranHealthEvent = "( -1 체력 )";

    private static TriggerManager instance;
    public static TriggerManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No TriggerManagerInstance");
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
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.IsContinue)
        {
            equips = SaveLoadManager.Instance.equips;
            truthPiece = SaveLoadManager.Instance.gambleItem;
            gambleItem = SaveLoadManager.Instance.gambleItem;
        }
    }

    public void CheckShowText(string[] showText)
    {
        if(GameManager.Instance.IsRebirth)
        {
            GameManager.Instance.IsRebirth = false;
            Underline.reDraw();
            return;
        }
        // 불러오기 했는데 -나 +가 포함되어 있으면 리턴
        else if (GameManager.Instance.IsContinue && !IsPreventDup)
        {
            IsPreventDup = true;
            return;
        }
        string text = "";
        for (int i = 0; i < showText.Length; i++)
        {
            text = showText[i];
            if ((IsAttackFail && text.Contains("체력")) && (TextManager.Instance.storyEventName == "R_3_A" || TextManager.Instance.storyEventName == "R_2_A"))
            {
                IsAttackFail = false;
                ranHealthEvent = "\r";
                text = ranHealthEvent;
                showText[i] = ranHealthEvent;
            }
            else if((!IsAttackFail && ranHealthEvent == "\r") && (TextManager.Instance.storyEventName == "R_3_A" || TextManager.Instance.storyEventName == "R_2_A"))
            {
                ranHealthEvent = "( -1 체력 )";
                text = ranHealthEvent;
                showText[i] = ranHealthEvent;
            }
            if (text.Contains("-") && text.Any(char.IsDigit))
            {
                //숫자만 자르기
                int count = int.Parse(Regex.Match(text, @"\d+").Value);

                //진실의 조각
                if (text.Contains("진실의 조각") && truthPiece - count >= 0)
                {
                    truthPiece -= count;
                    SaveLoadManager.Instance.truthPiece = truthPiece;
                    SaveLoadManager.Instance.SaveGameData();

                    audioSource.clip = minus;
                    audioSource.Play();
                }
                else
                {
                    truthPiece = 0;
                    SaveLoadManager.Instance.truthPiece = truthPiece;
                    SaveLoadManager.Instance.SaveGameData();

                    audioSource.clip = minus;
                    audioSource.Play();
                }

                // 플레이어 선택 아이템
                if (text.Contains("아까 플레이어가 고른 요소"))
                {
                    switch (gambleItem)
                    {
                        case 0:
                            Health.Instance.HealthMinus(1);
                            TextManager.Instance.talkDatas[0].showText[i] = TextManager.Instance.talkDatas[0].showText[i].Replace("(아까 플레이어가 고른 요소)", "체력");
                            break;
                        case 1:
                            Health.Instance.MentalMinus(1);
                            TextManager.Instance.talkDatas[0].showText[i] = TextManager.Instance.talkDatas[0].showText[i].Replace("(아까 플레이어가 고른 요소)", "정신력");
                            break;
                        case 2:
                            Health.Instance.CoinMinus(1);
                            TextManager.Instance.talkDatas[0].showText[i] = TextManager.Instance.talkDatas[0].showText[i].Replace("(아까 플레이어가 고른 요소)", "돈");
                            break;
                        default:
                            break;
                    }
                }
                if (text.Contains("체력")) Health.Instance.HealthMinus(count);
                else if (text.Contains("정신력")) Health.Instance.MentalMinus(count);
                else if (text.Contains("돈")) Health.Instance.CoinMinus(count);
            }
            else if (text.Contains("+") && text.Any(char.IsDigit))
            {
                //숫자만 자르기
                int count = int.Parse(Regex.Match(text, @"\d+").Value);

                if (text.Contains("진실의 조각") && truthPiece + count <= 5)
                {
                    truthPiece += count;
                    SaveLoadManager.Instance.truthPiece = truthPiece;
                    SaveLoadManager.Instance.SaveGameData();

                    audioSource.clip = plus;
                    audioSource.Play();
                }
                else
                {
                    truthPiece = 5;
                    SaveLoadManager.Instance.truthPiece = truthPiece;
                    SaveLoadManager.Instance.SaveGameData();

                    audioSource.clip = plus;
                    audioSource.Play();
                }

                if (text.Contains("체력")) Health.Instance.HealthPlus(count);
                else if (text.Contains("정신력")) Health.Instance.MentalPlus(count);
                else if (text.Contains("돈")) Health.Instance.CoinPlus(count);
            }
        }
        // 챕터 8 : 탈출도구
        if (TextManager.Instance.storyEventName == "8_2") equips[0] = true;
        else if (TextManager.Instance.storyEventName == "8_2_1") equips[1] = true;
        else if (TextManager.Instance.storyEventName == "8_2_8") equips[2] = true;
        SaveLoadManager.Instance.equips = equips;
        SaveLoadManager.Instance.SaveGameData();
    }
}
