using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class Underline : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TextMeshProUGUI tmpText;
    public RectTransform content;
    public static Action reDraw;

    [Header("Underline Settings")]
    [SerializeField] private Sprite underlineSprite;
    [SerializeField] private float offsetY = -40f;   // 줄 간격 (밑줄 간격)
    [SerializeField] private float height = 5f;      // 밑줄 두께
    [SerializeField] private int fixedLineCount = 6; // 항상 깔아둘 줄 개수
    private float fullWidth = 560;
    private float underLinePosX = 12.5f;
    private float prevHeight;

    private List<Image> underlineImages = new List<Image>();

    void Awake()
    {
        reDraw += ClearUnderlines;
        reDraw += SetFirstLine;
    }
    void Start()
    {
        SetFirstLine();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    // void OnEnable()
    // {
    //     scrollRect = FindObjectOfType<ScrollRect>();
    // }

    public void SetFirstLine()
    {
        float firstLine = 0f;

        // 미리 고정으로 그리는 줄(기본 6개)
        for (int i = 0; i < fixedLineCount; i++)
        {
            GameObject go = new GameObject("FixedUnderline", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(content, false);

            // 줄 이미지 설정
            Image img = go.GetComponent<Image>();
            img.sprite = underlineSprite;
            img.type = Image.Type.Sliced;
            Color imgCol = img.color;
            imgCol.a = 0.7f;
            img.color = imgCol;

            RectTransform rt = img.rectTransform;
            rt.sizeDelta = new Vector2(fullWidth, height);

            if (i == 0)
            {
                offsetY = tmpText.font.faceInfo.lineHeight * (tmpText.fontSize / tmpText.font.faceInfo.pointSize);
                firstLine = offsetY;
                rt.anchoredPosition = new Vector2(underLinePosX, -offsetY);
            }
            else if (i == 1)
            {
                offsetY = 42f;
                rt.anchoredPosition = new Vector2(underLinePosX, -(i + 1) * offsetY); // 일정 간격으로 밑줄 배치
            }
            else if (i == 2)
            {
                offsetY = 44f;
                rt.anchoredPosition = new Vector2(underLinePosX, -(i + 1) * offsetY); // 일정 간격으로 밑줄 배치
            }
            else if (i == 3)
            {
                offsetY = 44.5f;
                rt.anchoredPosition = new Vector2(underLinePosX, -(i + 1) * offsetY); // 일정 간격으로 밑줄 배치
            }
            else if (i == 4)
            {
                offsetY = 45f;
                rt.anchoredPosition = new Vector2(underLinePosX, -(i + 1) * offsetY); // 일정 간격으로 밑줄 배치
            }
            else if (i == 5)
            {
                offsetY = 45.5f;
                rt.anchoredPosition = new Vector2(underLinePosX, -(i + 1) * offsetY); // 일정 간격으로 밑줄 배치
            }
            underlineImages.Add(img);
            underlineImages[i].enabled = true;
        }
    }

    public void ClearUnderlines()
    {
        for (int i = 0; i < underlineImages.Count; i++)
        {
            if (underlineImages[i] != null)
                Destroy(underlineImages[i].gameObject);
        }

        underlineImages.Clear();
    }


    // 필요하다면 추가 줄을 동적으로 붙이는 로직
    void LateUpdate()
    {
        AutoScroll();
        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;

        int lineCount = textInfo.lineCount;

        // 초기 줄 이후의 추가 라인만 계산해서 붙이기
        for (int i = fixedLineCount; i < lineCount; i++)
        {
            if (i >= underlineImages.Count)
            {
                CreateUnderline();
            }

            TMP_LineInfo line = textInfo.lineInfo[i];
            if (line.characterCount == 0)
            {
                underlineImages[i].enabled = false;
                continue;
            }

            TMP_CharacterInfo firstChar = textInfo.characterInfo[line.firstCharacterIndex];
            offsetY = tmpText.font.faceInfo.lineHeight * (tmpText.fontSize / tmpText.font.faceInfo.pointSize);
            float y = firstChar.baseLine - Mathf.Abs(offsetY) * 0.2f; // baseline 보정

            RectTransform rt = underlineImages[i].rectTransform;
            rt.anchoredPosition = new Vector2(underLinePosX, y);
            rt.sizeDelta = new Vector2(fullWidth, height);

            underlineImages[i].enabled = true;
        }
    }

    private void CreateUnderline()
    {
        GameObject go = new GameObject("DynamicUnderline", typeof(RectTransform), typeof(Image));
        //go.transform.SetParent(content, false);
        go.transform.SetParent(tmpText.rectTransform, false);

        Image img = go.GetComponent<Image>();
        img.sprite = underlineSprite;
        img.type = Image.Type.Sliced;
        //img.enabled = false;
        Color imgCol = img.color;
        imgCol.a = 0.7f;
        img.color = imgCol;

        underlineImages.Add(img);
    }

    private void AutoScroll()
    {
        // content의 높이가 커졌는지 감지
        float currentHeight = tmpText.preferredHeight;
        if (currentHeight > prevHeight)
        {
            // 맨 아래로 자동 스크롤
            scrollRect.verticalNormalizedPosition = 0f;
            prevHeight = currentHeight;
        }
    }
}
