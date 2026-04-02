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

    [Header("Text Fade")]
    [SerializeField] private TextMeshProUGUI textMesh;

    // The speed at which the text fades in. Higher values result in faster fading.
    [SerializeField] private float fadeSpeed = 20.0f;

    // The number of characters affected at a time, creating a smoother transition effect.
    [SerializeField] private int characterSpread = 10;

    // Stores the running coroutine instance.
    private Coroutine _fadeCoroutine;
    private int _textCount;
    public TextMeshProUGUI TextMesh => textMesh;

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

        if (IsSkipDialog) characterTime = 0;
        else characterTime = timeForCharacter;
        if (dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray(); //받아온 다이얼 로그를 char로 변환.
            //StartCoroutine(Typer(chars, textObj)); //레퍼런스로 넘겨보는거 테스트 해보자.
            StartCoroutine(FadeText(chars,textObj));
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
    public IEnumerator FadeText(char[] dialog, TextMeshProUGUI textMesh)
    {
        int currentChar = 0;
        // Prepare the mesh & textInfo
        textMesh.text = dialog[currentChar].ToString();
        textMesh.ForceMeshUpdate();
        currentChar++;

        TMP_TextInfo textInfo = textMesh.textInfo;
        int totalChars = textInfo.characterCount;
        Color32[] newVertexColors = null;

        SetAllCharactersAlpha(0);

        // Compute one “step” of alpha change per frame
        byte fadeStep = (byte)Mathf.Max(1, 255 / characterSpread);

        int charsProcessed = 0;
        bool done = false;

        // Sweep across the characters
        while (!done)
        {
            for (int i = 0; i < charsProcessed + 1 && i < totalChars; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int matIdx = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIdx = textInfo.characterInfo[i].vertexIndex;
                newVertexColors = textInfo.meshInfo[matIdx].colors32;

                // Pick current alpha
                byte currentAlpha = newVertexColors[vertIdx].a;

                // Compute new alpha up or down
                int delta = +fadeStep;
                byte nextAlpha = (byte)Mathf.Clamp(currentAlpha + delta, 0, 255);

                // Apply to all four verts
                newVertexColors[vertIdx + 0].a = nextAlpha;
                newVertexColors[vertIdx + 1].a = nextAlpha;
                newVertexColors[vertIdx + 2].a = nextAlpha;
                newVertexColors[vertIdx + 3].a = nextAlpha;
            }

            // Push to mesh
            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // Advance the sweep
            if (charsProcessed < totalChars)
                charsProcessed++;

            // Detect completion on the last character
            if (charsProcessed >= totalChars && newVertexColors != null)
            {
                TMP_CharacterInfo lastChar = textInfo.characterInfo[totalChars - 1];
                int finalAlpha = newVertexColors[lastChar.vertexIndex].a;
                done = finalAlpha == 255;
            }

            // Wait just like your old methods
            yield return new WaitForSeconds(0.02f + (0.25f - fadeSpeed * 0.01f));
        }
    }

    /// <summary>
    /// Sets every visible character’s vertex alpha to the given value (0–255).
    /// </summary>
    /// <param name="alpha">
    /// The alpha value to apply to all characters (0 = fully transparent, 255 = fully opaque).
    /// </param>
    public void SetAllCharactersAlpha(byte alpha)
    {
        // Rebuild the mesh so textInfo and meshInfo are valid
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        // Loop each character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int matIdx = charInfo.materialReferenceIndex;
            var verts = textInfo.meshInfo[matIdx].colors32;
            int vIdx = charInfo.vertexIndex;

            // Set all four vertices to the requested alpha
            verts[vIdx + 0].a = alpha;
            verts[vIdx + 1].a = alpha;
            verts[vIdx + 2].a = alpha;
            verts[vIdx + 3].a = alpha;
        }

        // Push the updated colors back into each mesh
        for (int m = 0; m < textInfo.meshInfo.Length; m++)
            textInfo.meshInfo[m].mesh.colors32 = textInfo.meshInfo[m].colors32;
    }
}