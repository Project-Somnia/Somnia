using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Underline : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    [Header("Underline Settings")]
    [SerializeField] private Sprite underlineSprite;
    [SerializeField] private float offsetY = -10f;   // 텍스트 밑으로 얼마나 내릴지
    [SerializeField] private float height = 5f;      // 밑줄 두께

    private List<Image> underlineImages = new List<Image>();

    void Awake()
    {
        //tmpText = GetComponent<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;

        int lineCount = textInfo.lineCount;

        // 밑줄 오브젝트 수 맞추기
        while (underlineImages.Count < lineCount)
        {
            GameObject go = new GameObject("Underline", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);

            Image img = go.GetComponent<Image>();
            img.sprite = underlineSprite;
            img.type = Image.Type.Sliced;
            underlineImages.Add(img);
        }
        for (int i = lineCount; i < underlineImages.Count; i++)
        {
            underlineImages[i].enabled = false; // 필요 없는 건 숨김
        }

        // 각 줄마다 밑줄 배치
        for (int i = 0; i < lineCount; i++)
        {
            TMP_LineInfo line = textInfo.lineInfo[i];

            if (line.characterCount == 0)
            {
                underlineImages[i].enabled = false;
                continue;
            }

            // 줄의 첫 번째 문자의 baseline 기준으로 y 좌표 가져오기
            TMP_CharacterInfo firstChar = textInfo.characterInfo[line.firstCharacterIndex];
            Vector3 baseline = firstChar.bottomLeft;

            // 텍스트 로컬 좌표 → 월드 좌표 → 다시 로컬로 변환
            Vector3 worldBL = tmpText.transform.TransformPoint(baseline);
            Vector3 localBL = transform.InverseTransformPoint(worldBL);

            // 텍스트 전체 폭
            float fullWidth = tmpText.rectTransform.rect.width;

            RectTransform rt = underlineImages[i].rectTransform;
            rt.anchoredPosition = new Vector2(0, localBL.y + offsetY); // X는 중앙 기준
            rt.sizeDelta = new Vector2(fullWidth, height);

            underlineImages[i].enabled = true;
        }

    }
}