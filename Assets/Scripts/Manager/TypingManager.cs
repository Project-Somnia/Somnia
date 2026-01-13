using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingManager : MonoBehaviour
{
    private static TypingManager instance;
    public static TypingManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No TypingInstance");
            }
            return instance;
        }
    }

    [Header("Times for each character")]
    public float timeForCharacter; //기본.

    [Header("Times for each character when speed up")]
    public float timeForCharacter_Fast; //빠른 텍스트.
    public float timeForCharacter_Very_Fast; //제일 빠른 텍스트.

    float characterTime; // 실제 적용되는 문자열 속도.

    //임시 저장되는 대화 오브젝트와 대화내용.
    string[] dialogsSave;
    TextMeshProUGUI tmpSave;

    public bool isDialogEnd;
    public bool isTypingEnd = false; //타이핑이 끝났는가?
    public bool IsSkipDialog = false;
    int dialogNumber = 0; //대화 문단 숫자.

    float timer; //내부적으로 돌아가는 시간 타이머

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timer = timeForCharacter;
        characterTime = timeForCharacter;
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj)
    {
        isDialogEnd = false;
        dialogsSave = dialogs;

        tmpSave = textObj;

        if(IsSkipDialog) characterTime = 0;
        else characterTime = timeForCharacter;
        if (dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray(); //받아온 다이얼 로그를 char로 변환.
            StartCoroutine(Typer(chars, textObj)); //레퍼런스로 넘겨보는거 테스트 해보자.
        }
        else
        {
            isDialogEnd = true; // 호출자는 다이알로그 엔드를 보고 다음 동작을 진행해주면 됨.
            dialogsSave = null;
            dialogNumber = 0;
        }
    }

    public void GetInputDown()
    {
        //인풋이 들어왔을때 -> 텍스트가 진행중이면 빠르게 진행되고 텍스트가 마감되어있으면 다음 텍스트로 넘어감.
        //그리고 인풋이 캔슬되면 다시 문자열 속도를 정상화 시켜야함.
        if (dialogsSave != null)
        {
            if (isTypingEnd)
            {
                GetInputUp();
                Typing(dialogsSave, tmpSave);
            }
            // if(TextManager.Instance.charSpeedLevel == 0) characterTime = timeForCharacter; //기본
            // else if(TextManager.Instance.charSpeedLevel == 1) characterTime = timeForCharacter_Fast; //기본
            // else if(TextManager.Instance.charSpeedLevel == 2) characterTime = timeForCharacter_Very_Fast; //기본
            if (IsSkipDialog) characterTime = 0;
            else characterTime = timeForCharacter;
        }
    }

    public void GetInputUp()
    {
        //인풋이 끝났을때.
        if (dialogsSave != null)
        {
            characterTime = timeForCharacter;
        }
    }

    IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        isTypingEnd = false;

        while (currentChar < charLength)
        {
            if (timer > 0)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            else
            {
                textObj.text += chars[currentChar].ToString();
                currentChar++;
                timer = characterTime; //타이머 초기화
            }
        }
        if (currentChar >= charLength)
        {
            isTypingEnd = true;
            dialogNumber++;
            yield break;
        }
    }
}