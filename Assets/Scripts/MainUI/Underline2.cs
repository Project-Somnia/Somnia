using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Underline2 : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public RectTransform content; // content (ScrollRect의 Content)
    
    [Header("Underline Settings")]
    [SerializeField] private Sprite underlineSprite;
    [SerializeField] private float height = 5f;      // 밑줄 두께
    [SerializeField] private int fixedLineCount = 6; // 항상 깔아둘 줄 개수
    [SerializeField] private float fullWidth = 540f;
    [SerializeField] private float underLinePosX = 12.5f;
    [SerializeField] private float underlineBelowDescent = 2f; // 밑줄을 descent 아래로 얼마나 더 둘지(조정용)

    private List<Image> underlineImages = new List<Image>();

    void Start()
    {
        // 밑줄을 tmpText의 좌표계에 붙이는 게 가장 안전합니다.
        // (content에 붙여도 되지만 tmpText와 동일 좌표계가 보장되어야 함)
        // 여기서는 tmpText 내부 로컬 좌표계를 사용합니다.
        // (만약 content 좌표계로 붙이고 싶으면 content를 사용하고 Transform 변환을 조정하세요)
        for (int i = 0; i < fixedLineCount; i++)
        {
            GameObject go = new GameObject($"FixedUnderline_{i}", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(tmpText.rectTransform, false);

            Image img = go.GetComponent<Image>();
            img.sprite = underlineSprite;
            img.type = Image.Type.Sliced;
            Color imgCol = img.color;
            imgCol.a = 0.7f;
            img.color = imgCol;

            RectTransform rt = img.rectTransform;
            rt.sizeDelta = new Vector2(fullWidth, height);

            underlineImages.Add(img);
            img.enabled = true;
        }

        // 초기 배치 (텍스트가 비어있어도 폰트 메트릭으로 위치 계산)
        UpdateFixedUnderlines();
    }

    void LateUpdate()
    {
        // 텍스트가 타이핑 되는 중에도 매 프레임 위치 보정
        UpdateFixedUnderlines();
        UpdateDynamicUnderlines();
    }

    // 고정된 N줄(초기 줄) 배치
    private void UpdateFixedUnderlines()
    {
        tmpText.ForceMeshUpdate(); // 최신 레이아웃으로 갱신
        TMP_TextInfo tInfo = tmpText.textInfo;

        // 폰트 스케일(현재 fontSize에 대한 비율)
        float scale = tmpText.fontSize / tmpText.font.faceInfo.pointSize;

        // 우선 가능한 정확한 lineHeight 가져오기 (있으면 사용)
        float lineHeight = 0f;
        if (tInfo.lineCount > 0)
        {
            // 실제 계산된 lineHeight 사용 (더 정확)
            lineHeight = tInfo.lineInfo[0].lineHeight;
        }
        else
        {
            // 없으면 폰트 메트릭으로 추정
            lineHeight = tmpText.font.faceInfo.lineHeight * scale;
        }

        // ascent/descent (scaled)
        float ascent = tmpText.font.faceInfo.ascentLine * scale;
        float descent = tmpText.font.faceInfo.descentLine * scale;

        // tmpText의 로컬 좌표계에서 "위쪽(텍스트 영역의 top)" 위치 계산
        // Rect의 top Y in local space (RectTransform's local coordinates: pivot considered)
        Rect rect = tmpText.rectTransform.rect;
        float topLocalY = rect.height * (1f - tmpText.rectTransform.pivot.y); // 예: pivot.y = 1이면 topLocalY = 0

        // baseline 위치 (local Y) 계산: top에서 ascent만큼 아래로
        float baselineLocalY = topLocalY - ascent;

        // 밑줄을 baseline 아래 (descent + 추가 여유) 위치에 둡니다.
        // first underline 위치
        float firstUnderlineY = baselineLocalY - descent - underlineBelowDescent;

        // 이제 고정된 줄들을 lineHeight 간격으로 아래로 배치
        for (int i = 0; i < fixedLineCount; i++)
        {
            if (i >= underlineImages.Count) break;
            RectTransform rt = underlineImages[i].rectTransform;
            float y = firstUnderlineY - (i * lineHeight);
            rt.anchoredPosition = new Vector2(underLinePosX, y);
            rt.sizeDelta = new Vector2(fullWidth, height);
            underlineImages[i].enabled = true;
        }
    }

    // 텍스트가 실제로 늘어나 추가 라인이 필요할 때 생성/배치
    private void UpdateDynamicUnderlines()
    {
        tmpText.ForceMeshUpdate();
        TMP_TextInfo tInfo = tmpText.textInfo;
        int totalLines = tInfo.lineCount;

        // 필요한 갯수만큼 생성
        while (underlineImages.Count < totalLines)
            CreateUnderline();

        for (int i = underlineImages.Count - 1; i >= 0; i--)
        {
            // i번째 라인이 실제로 존재하면 맞춰주고, 없으면 숨김처리
            if (i < totalLines)
            {
                // 각 라인의 baseline을 직접 읽을 수 있으면 그걸 사용 (더 정확)
                TMP_LineInfo lineInfo = tInfo.lineInfo[i];
                if (lineInfo.characterCount > 0)
                {
                    TMP_CharacterInfo firstChar = tInfo.characterInfo[lineInfo.firstCharacterIndex];
                    // bottomLeft는 문자 하단 로컬 좌표이므로, 여기에 살짝 아래로 보정
                    Vector3 worldBL = tmpText.transform.TransformPoint(firstChar.bottomLeft);
                    Vector3 localBL = tmpText.rectTransform.InverseTransformPoint(worldBL);
                    float y = localBL.y - (height * 0.5f); // 밑줄을 문자 바로 아래에 놓기
                    RectTransform rt = underlineImages[i].rectTransform;
                    rt.anchoredPosition = new Vector2(underLinePosX, y);
                    rt.sizeDelta = new Vector2(fullWidth, height);
                    underlineImages[i].enabled = true;
                    continue;
                }
            }

            // 해당 라인이 없거나 character가 없으면 숨기기 (단, fixedLine 범위 내는 유지)
            if (i < fixedLineCount)
            {
                underlineImages[i].enabled = true; // 고정 라인은 항상 보이게
            }
            else
            {
                underlineImages[i].enabled = false;
            }
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
