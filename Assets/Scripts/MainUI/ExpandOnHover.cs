using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ExpandLeftOnly : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Panel")]
    [SerializeField] private RectTransform panel; // 앵커=(0.5,0.5), 피벗=(0.5,0.5)

    [Header("Children (auto find if empty)")]
    [SerializeField] private RectTransform imageRT;   // Panel/Image
    [SerializeField] private RectTransform buttonRT;  // Panel/AdButton
    [SerializeField] private bool autoMeasure = true;

    [Header("Sizing")]
    [SerializeField] private float imageWidth = 50f;
    [SerializeField] private float buttonWidth = 160f;
    [SerializeField] private float rightPadding = 16f;
    [SerializeField] private float spacing = 20f;
    [SerializeField] private float collapsedWidth = 0f; // 자동 계산됨
    [SerializeField] private float expandedWidth  = 0f; // 자동 계산됨
    [SerializeField] private float offscreenExtra = 40f; // 버튼을 화면 밖으로 더 보내는 여유

    [Header("Tween")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private float exitDelay = 0.12f;

    // 자식 이동도 같이 트윈
    private Tween widthTween;
    private Tween imageTween;
    private Tween buttonTween;

    private bool hovered;
    private float exitTimer;

    private float rightEdgeX;

    // 자식 X 목표값(오른쪽 앵커/피벗 기준)
    private float imageX_Collapsed;
    private float imageX_Expanded;
    private float buttonX_Hidden;   // 오른쪽 바깥
    private float buttonX_Shown;    // 패널 안(오른쪽)

    private void Reset()
    {
        panel = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        if (!panel) panel = GetComponent<RectTransform>();

        if (!imageRT)  imageRT  = transform.Find("Image")?.GetComponent<RectTransform>();
        if (!buttonRT) buttonRT = transform.Find("AdButton")?.GetComponent<RectTransform>();

        // 레이아웃/텍스트 반영
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

        // 현재 오른쪽 가장자리 기록(중앙 피벗 0.5 전제)
        rightEdgeX = panel.anchoredPosition.x + panel.rect.width * 0.5f;

        // 폭 계산
        RecalculateWidths();

        // 앵커/피벗 정렬 + 자식 위치 세팅(핵심)
        SetupChildPositions();

        // 접힌 상태 적용
        SetWidthKeepRight(collapsedWidth);
        SetChildrenX(imageX_Collapsed, buttonX_Hidden);
    }

    private void Update()
    {
        if (!hovered && exitTimer > 0f)
        {
            exitTimer -= Time.unscaledDeltaTime;
            if (exitTimer <= 0f)
            {
                AnimateTo(collapsedWidth, imageX_Collapsed, buttonX_Hidden);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        exitTimer = 0f;

        AnimateTo(expandedWidth, imageX_Expanded, buttonX_Shown);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        exitTimer = exitDelay;
    }

    private void AnimateTo(float targetWidth, float imageTargetX, float buttonTargetX)
    {
        widthTween?.Kill();
        imageTween?.Kill();
        buttonTween?.Kill();

        // 패널 폭
        float startW = panel.rect.width;
        widthTween = DOTween.To(() => startW, w => SetWidthKeepRight(w), targetWidth, duration)
                            .SetEase(ease)
                            .SetUpdate(true);

        // Image 이동(왼쪽으로 밀림)
        float imgStartX = imageRT.anchoredPosition.x;
        imageTween = DOTween.To(() => imgStartX, x => SetImageX(x), imageTargetX, duration)
                            .SetEase(ease)
                            .SetUpdate(true);

        // Button 이동(오른쪽 밖 -> 안으로 들어옴 / 또는 반대)
        float btnStartX = buttonRT.anchoredPosition.x;
        buttonTween = DOTween.To(() => btnStartX, x => SetButtonX(x), buttonTargetX, duration)
                             .SetEase(ease)
                             .SetUpdate(true);
    }

    private void RecalculateWidths()
    {
        if (!autoMeasure || !imageRT || !buttonRT)
        {
            collapsedWidth = imageWidth + rightPadding * 2f;
            expandedWidth  = imageWidth + spacing + buttonWidth + rightPadding;
            return;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

        imageWidth = imageRT.rect.width;
        buttonWidth = buttonRT.rect.width;

        collapsedWidth = imageWidth + rightPadding * 2f;
        expandedWidth  = imageWidth + spacing + buttonWidth + rightPadding;
    }

    private void SetupChildPositions()
    {
        if (!imageRT || !buttonRT) return;

        // 둘 다 오른쪽 기준으로 고정(스샷처럼 이미 맞춰져 있어도 OK)
        SetRightAnchorPivot(imageRT);
        SetRightAnchorPivot(buttonRT);

        // ✅ 목표 X 계산(오른쪽 앵커/피벗 기준)
        // 접힘: 이미지가 오른쪽에 붙어서 보임
        imageX_Collapsed = -rightPadding;

        // 펼침: 버튼이 오른쪽 자리를 차지 -> 이미지는 버튼+spacing만큼 왼쪽으로 밀림
        imageX_Expanded  = -(buttonWidth + spacing);

        // 버튼은 "오른쪽 바깥"에서 시작해서
        buttonX_Hidden = buttonWidth + rightPadding + offscreenExtra;  // 패널 오른쪽 밖(양수면 오른쪽으로 나감)

        // 펼치면 버튼이 패널 오른쪽 안쪽에 위치
        buttonX_Shown  = 0f;
    }

    private void SetRightAnchorPivot(RectTransform rt)
    {
        rt.anchorMin = new Vector2(1f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot     = new Vector2(1f, 0.5f);
    }

    private void SetChildrenX(float imageX, float buttonX)
    {
        SetImageX(imageX);
        SetButtonX(buttonX);
    }

    private void SetImageX(float x)
    {
        var p = imageRT.anchoredPosition;
        p.x = x;
        imageRT.anchoredPosition = p;
    }

    private void SetButtonX(float x)
    {
        var p = buttonRT.anchoredPosition;
        p.x = x;
        buttonRT.anchoredPosition = p;
    }

    /// <summary>
    /// 폭을 w로 바꾸되, 오른쪽 가장자리를 고정하여 '왼쪽으로만' 확장
    /// (Panel 앵커/피벗 = 중앙(0.5,0.5) 전제)
    /// </summary>
    private void SetWidthKeepRight(float w)
    {
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        Vector2 pos = panel.anchoredPosition;
        pos.x = rightEdgeX - w * 0.5f;
        panel.anchoredPosition = pos;
    }
}
