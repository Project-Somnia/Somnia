using UnityEngine;
using TMPro;

public class ScrollText : MonoBehaviour
{

    public TextMeshProUGUI tmpText;
    public RectTransform underlinePrefab;

    private int lastLineCount = 0;

    void Update()
    {
        tmpText.ForceMeshUpdate();
        int currentLineCount = tmpText.textInfo.lineCount;

        if (currentLineCount > lastLineCount)
        {
            for (int i = lastLineCount; i < currentLineCount; i++)
            {
                AddUnderline(i);
            }
            lastLineCount = currentLineCount;
        }
    }

    void AddUnderline(int lineIndex)
    {
        TMP_LineInfo lineInfo = tmpText.textInfo.lineInfo[lineIndex];

        float startX = lineInfo.lineExtents.min.x;
        float endX   = lineInfo.lineExtents.max.x;

        // baseline 기준으로 조금 아래쪽
        float yPos = lineInfo.baseline - (tmpText.fontSize * 0.25f);

        // ==== TMP localPosition을 부모(Content) 기준으로 변환 ====
        Vector3 worldPos = tmpText.transform.TransformPoint(new Vector3((startX + endX) / 2f, yPos, 0));
        Vector3 localPos = tmpText.transform.parent.InverseTransformPoint(worldPos);

        // 밑줄 생성 (Text의 부모 = Content 밑에 붙인다)
        RectTransform underline = Instantiate(underlinePrefab, tmpText.transform.parent);
        underline.name = $"Underline_{lineIndex}";
        underline.localScale = Vector3.one;

        // 위치/크기 세팅
        underline.localPosition = localPos;
        underline.sizeDelta = new Vector2(endX - startX, underline.sizeDelta.y);
    }
}