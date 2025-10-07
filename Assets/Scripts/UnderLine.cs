using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Underline : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public RectTransform content;

    [Header("Underline Settings")]
    [SerializeField] private Sprite underlineSprite;
    [SerializeField] private float offsetY = -40f;   // 줄 간격 (밑줄 간격)
    [SerializeField] private float height = 5f;      // 밑줄 두께
    [SerializeField] private int fixedLineCount = 6; // 항상 깔아둘 줄 개수
    private float fullWidth = 540;
    private float underLinePosX = 12.5f;

    private List<Image> underlineImages = new List<Image>();

    void Start()
    {

        // 8줄은 게임 시작 시 미리 고정으로 그림
        for (int i = 0; i < fixedLineCount; i++)
        {
            GameObject go = new GameObject("FixedUnderline", typeof(RectTransform), typeof(Image));
            //go.transform.SetParent(tmpText.rectTransform, false);
            go.transform.SetParent(content, false);

            Image img = go.GetComponent<Image>();
            img.sprite = underlineSprite;
            img.type = Image.Type.Sliced;

            RectTransform rt = img.rectTransform;
            rt.sizeDelta = new Vector2(fullWidth, height);
            rt.anchoredPosition = new Vector2(underLinePosX, -(i+1) * offsetY); // 일정 간격으로 밑줄 배치

            underlineImages.Add(img);
            underlineImages[i].enabled = true;
        }
    }

    // 필요하다면 추가 줄을 동적으로 붙이는 로직
    void LateUpdate()
    {
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
            float y = firstChar.baseLine - Mathf.Abs(offsetY) * 0.2f; // baseline 보정

            RectTransform rt = underlineImages[i].rectTransform;
            rt.anchoredPosition = new Vector2(0, y);
            rt.sizeDelta = new Vector2(fullWidth, height);

            underlineImages[i].enabled = true;
        }
    }

    private void CreateUnderline()
    {
        GameObject go = new GameObject("DynamicUnderline", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(tmpText.rectTransform, false);

        Image img = go.GetComponent<Image>();
        img.sprite = underlineSprite;
        img.type = Image.Type.Sliced;
        img.enabled = false;

        underlineImages.Add(img);
    }
}
