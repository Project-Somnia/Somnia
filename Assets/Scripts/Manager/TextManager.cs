using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TextManager : MonoBehaviour
{
    public GameObject ChapterObject;
    public GameObject commingsoon;
    public GameObject textLogBox;
    public Transform contentTr;
    public GameOverUI gameOverUI;
    public TextMeshProUGUI storyText;
    public ScrollRect logScrollRect;
    public ScrollRect storyScrollRect;

    string[] dialogStrings;
    public TalkData[] talkDatas;
    public string storyEventName;


    public string[] selectText = new string[3];
    public string[] triggerEvent = new string[3];
    public string showTextDup;

    public int currentPage = 0; // 대화문 개수 변수
    private int curDialog = 0;
    public string pendingMainEventId;
    public bool IsStory = false;
    public bool IsDialogSet = false;
    public int charSpeedLevel = 0;
    public string originNextEvent = "";
    public string curSplitEvent = "1";
    private float scrollSensitivity = 10f;
    private bool IsTextFadeEnd = false;
    private bool IsDialogEnd = false;
    public bool IsSkipDialog = false;

    [Header("Text Fade")]

    // The speed at which the text fades in. Higher values result in faster fading.
    [SerializeField] private float fadeSpeed = 50.0f;

    // The number of characters affected at a time, creating a smoother transition effect.
    [SerializeField] private int characterSpread = 20;

    // Stores the running coroutine instance.
    private Coroutine _fadeCoroutine;
    private int _textCount;

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

    void Start()
    {
        StartCoroutine("WaitAndSet");
        logScrollRect.movementType = ScrollRect.MovementType.Clamped;
        logScrollRect.scrollSensitivity = scrollSensitivity;
    }

    private void Update()
    {
        if (IsStory)
        {
            TypingManager.Instance.GetInputDown();
            if (IsSkipDialog)
            {
                IsDialogEnd = true;
                IsTextFadeEnd = true;
                IsSkipDialog = false;
                StopCoroutine("FadeText");
                SetAllCharactersAlpha(255);
                Color color = storyText.color;
                color.a = 1f;
                storyText.color = color;
                storyScrollRect.verticalNormalizedPosition = 0f;
            }
            //if (TypingManager.Instance.isTypingEnd)
            if (IsTextFadeEnd)
            {
                //if (currentPage == talkDatas.Length && TypingManager.Instance.isDialogEnd)
                if (currentPage == talkDatas.Length && IsDialogEnd)
                {
                    TypingManager.Instance.IsSkipDialog = false;
                    IsStory = false;
                    //TypingManager.Instance.IsSkipDialog = false;
                    if (!GameManager.Instance.IsGameOver)
                    {
                        StoryChoice.fadeChoice();
                        currentPage = 0;
                        return;
                    }
                    if (GameManager.Instance.IsGameOver && !GameManager.Instance.IsMentalMor)
                    {
                        gameOverUI.ShowHpGameOver();
                    }
                    else if (GameManager.Instance.IsGameOver && GameManager.Instance.IsMentalMor)
                    {
                        gameOverUI.ShowMentalGameOver();
                    }
                    //게임오버상태면 선택지 표시하지 않음
                }
            }
        }
    }

    public void SetDialogue(string eventNumber)
    {
        storyText.text = "";
        storyEventName = eventNumber;

        IsDialogEnd = false;
        curDialog = 0;

        // 텍스트 정보 가져오기
        talkDatas = this.GetComponent<Dialogue>().GetObjectDialogue();
        TriggerManager.Instance.CheckShowText(talkDatas[0].showText);
        // 한번 더 체크
        if (GameManager.Instance.IsZero)
        {
            if (Health.Instance.health == 0 && !GameManager.Instance.IsGameOver)
            {
                GameManager.Instance.IsGameOver = true;
                SetDialogue("D_1");
                return;
            }

            else if (Health.Instance.mental == 0 && !GameManager.Instance.IsMentalMor)
            {
                GameManager.Instance.IsMentalMor = true;
                SetDialogue("T_1");
                return;
            }
            else if (storyEventName == "T_4") GameManager.Instance.IsGameOver = true;
        }

        //오디오 세팅
        AudioManager.Instance.CheckAudioStat(storyEventName.Split('_')[0]);
        // 사진 세팅
        PaintManager.Instance.SetPaint(talkDatas[0].eventImage);
        //TypingManager.Instance.Typing(talkDatas[0].showText, storyText);

        for (int i = 0; i < talkDatas[0].showText.Length; i++)
        {
            storyText.text += talkDatas[0].showText[i];
        }
        StartCoroutine("FadeText");

        IsDialogEnd = true;
        currentPage++;

        //로그에 기록
        GameObject newLog = Instantiate(textLogBox, contentTr);
        TextMeshProUGUI textLog = newLog.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < talkDatas[0].showText.Length; i++) textLog.text += talkDatas[0].showText[i];
        logScrollRect.verticalNormalizedPosition = 0f;

        // 선택지 세팅
        selectText[0] = talkDatas[0].selectText1;
        selectText[1] = talkDatas[0].selectText2;
        selectText[2] = talkDatas[0].selectText3;

        triggerEvent[0] = talkDatas[0].triggerEvent1;
        triggerEvent[1] = talkDatas[0].triggerEvent2;
        triggerEvent[2] = talkDatas[0].triggerEvent3;

        StoryChoice.setChoiceText();
        IsStory = true;

        SaveLoadManager.Instance.eventNumber = eventNumber;
        SaveLoadManager.Instance.SaveGameData();

        // 스토리 텍스트 확인
        showTextDup = talkDatas[0].showText[0];
    }

    public void SetDialogueFromChoice(string triggerEventId)
    {
        if (triggerEventId == "RETURN_MAIN")
        {
            if (GameManager.Instance.IsMentalMor)
            {
                GameManager.Instance.IsZero = false;
                GameManager.Instance.IsMentalMor = false;
                GameManager.Instance.IsRebirth = true;

                pendingMainEventId = originNextEvent;
                curSplitEvent = storyEventName.Split("_")[0];
                SetDialogue(originNextEvent);
                return;
            }
            else if (!string.IsNullOrEmpty(pendingMainEventId))
            {
                string mainId = pendingMainEventId;

                // 한 번 쓰면 비워준다 (다음 랜덤에 영향 없게)
                pendingMainEventId = null;
                curSplitEvent = storyEventName.Split("_")[0];
                SetDialogue(mainId);
                return;
            }
            else
            {
                Debug.LogError("RETURN_MAIN인데 pendingMainEventId가 비어 있음");
                return;
            }
        }
        string finalId = triggerEventId;

        // EncounterFlowManager가 존재하면 랜덤 인카운터 로직 적용
        if (EncounterFlowManager.Instance != null && !GameManager.Instance.IsZero)
        {
            // storyEventName = 현재 진행 중인 메인/랜덤 이벤트
            finalId = EncounterFlowManager.Instance.DecideNextEvent(storyEventName, triggerEventId);
        }
        SetDialogue(finalId);
    }

    IEnumerator WaitAndSet()
    {
        while (!IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator FadeText()
    {
        // 1. 현재 대화 인덱스 체크 (안전 장치)
        if (curDialog >= talkDatas[0].showText.Length)
        {
            IsDialogEnd = true;
            yield break;
        }

        IsTextFadeEnd = false;

        // 2. 텍스트 초기화 및 강제 업데이트
        Color color = storyText.color;
        color.a = 0f;
        storyText.color = color;
        storyText.ForceMeshUpdate(true);

        TMP_TextInfo textInfo = storyText.textInfo;
        int totalChars = textInfo.characterCount;

        // 3. 모든 문자를 투명하게 (초기화)
        SetAllCharactersAlpha(0);
        yield return null; // 메쉬 안정화를 위한 1프레임 대기

        // ---------------------------------------------------------
        // [중요] 실제로 눈에 보이는 마지막 글자의 인덱스를 미리 찾습니다.
        // 문장 끝에 공백(Space)이나 줄바꿈이 있을 경우를 대비합니다.
        int lastVisibleCharIndex = -1;
        for (int i = totalChars - 1; i >= 0; i--)
        {
            if (textInfo.characterInfo[i].isVisible)
            {
                lastVisibleCharIndex = i;
                break;
            }
        }
        storyScrollRect.verticalNormalizedPosition = 1f;
        // ---------------------------------------------------------

        byte fadeStep = (byte)Mathf.Max(1, 255 / characterSpread);
        int charsProcessed = 0;
        bool done = false;
        Vector3[] viewportCorners = new Vector3[4];

        // 4. 페이드 효과 루프
        while (!done)
        {
            // 보여줄 글자가 하나도 없다면(공백만 있는 경우 등) 즉시 종료
            if (lastVisibleCharIndex == -1)
            {
                done = true;
                break;
            }

            // 각 글자의 알파값 증가 로직
            for (int i = 0; i < charsProcessed + 1 && i < totalChars; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int matIdx = charInfo.materialReferenceIndex;
                int vertIdx = charInfo.vertexIndex;

                Color32[] newVertexColors = textInfo.meshInfo[matIdx].colors32;

                byte currentAlpha = newVertexColors[vertIdx].a;

                // 이미 255면 연산 건너뛰기 (최적화)
                if (currentAlpha == 255) continue;

                byte nextAlpha = (byte)Mathf.Clamp(currentAlpha + fadeStep, 0, 255);

                for (int j = 0; j < 4; j++)
                {
                    newVertexColors[vertIdx + j].a = nextAlpha;
                }
            }

            storyText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            if (charsProcessed < totalChars)
            {
                storyScrollRect.viewport.GetWorldCorners(viewportCorners);
                float viewportBottomY = viewportCorners[0].y; // 뷰포트의 하단 Y 좌표

                // 2. 현재 처리 중인 글자의 정보를 가져옵니다.
                TMP_CharacterInfo cInfo = textInfo.characterInfo[charsProcessed];
                if (cInfo.isVisible)
                {
                    // 3. 글자의 왼쪽 아래(BottomLeft) 좌표를 월드 좌표로 변환
                    // (characterInfo의 좌표는 텍스트 오브젝트 기준 로컬 좌표이므로 변환 필요)
                    Vector3 charBottomPos = storyText.transform.TransformPoint(cInfo.bottomLeft);

                    // 4. 글자의 바닥이 뷰포트 바닥보다 아래에 있다면? -> 스크롤 내림
                    if (charBottomPos.y < viewportBottomY)
                    {
                        //storyScrollRect.verticalNormalizedPosition = 0f;
                        storyScrollRect.verticalNormalizedPosition = Mathf.Lerp(storyScrollRect.verticalNormalizedPosition, 0f, Time.deltaTime * 10f);
                        // 만약 너무 딱딱하게 내려가는 게 싫다면 Lerp 사용 (선택 사항)
                        // scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0f, Time.deltaTime * 10f);
                    }
                }
                charsProcessed++;
            }

            // ---------------------------------------------------------
            // [탈출 조건 수정]
            // 처리된 글자 순서(charsProcessed)가 실제 마지막 글자 순서를 지났는지 확인하고,
            // 실제 마지막 글자의 알파값이 255(완전 불투명)가 되었는지 확인합니다.
            // ---------------------------------------------------------
            if (charsProcessed > lastVisibleCharIndex)
            {
                var lastCharInfo = textInfo.characterInfo[lastVisibleCharIndex];
                int mIdx = lastCharInfo.materialReferenceIndex;
                int vIdx = lastCharInfo.vertexIndex;

                // 실제 메쉬 데이터에서 알파값 확인
                byte finalAlpha = textInfo.meshInfo[mIdx].colors32[vIdx].a;

                if (finalAlpha >= 255)
                {
                    done = true;
                }
            }
            storyText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            //storyScrollRect.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(0.02f);
        }
        // 5. 후처리
        IsTextFadeEnd = true;
        curDialog++;
    }

    /// <summary>
    /// Sets every visible character’s vertex alpha to the given value (0–255).
    /// </summary>
    /// <param name="alpha">
    /// The alpha value to apply to all characters (0 = fully transparent, 255 = fully opaque).
    /// </param>
    public void SetAllCharactersAlpha(byte alpha)
    {
        storyText.ForceMeshUpdate(true);
        TMP_TextInfo textInfo = storyText.textInfo;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            Color32[] colors = textInfo.meshInfo[i].colors32;
            for (int j = 0; j < colors.Length; j++)
            {
                colors[j].a = alpha;
            }
        }
        storyText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

}