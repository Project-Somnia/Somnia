using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ExpandLeftOnly : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform panel;        // 앵커=(0.5,0.5), 피벗=(0.5,0.5)
    [SerializeField] private float collapsedWidth = 0f;
    [SerializeField] private float expandedWidth  = 300f;
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private float exitDelay = 0.12f;

    private Tween tween;
    private bool hovered;
    private float exitTimer;

    // 고정할 '오른쪽 가장자리'의 로컬 X 좌표(부모 기준)
    private float rightEdgeX;

    private void Reset()
    {
        panel = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        if (!panel) panel = GetComponent<RectTransform>();

        // 시작은 접힌 상태
        SetWidthKeepRight(collapsedWidth, init:true);
        // 현재 상태에서 오른쪽 가장자리 좌표를 기록 (앵커/피벗=중앙 가정)
        rightEdgeX = panel.anchoredPosition.x + panel.rect.width * 0.5f;
    }

    private void Update()
    {
        if (!hovered && exitTimer > 0f)
        {
            exitTimer -= Time.unscaledDeltaTime;
            if (exitTimer <= 0f) AnimateTo(collapsedWidth);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        exitTimer = 0f;
        AnimateTo(expandedWidth);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        exitTimer = exitDelay;
    }

    private void AnimateTo(float targetWidth)
    {
        tween?.Kill();

        float startW = panel.rect.width;
        tween = DOTween.To(() => startW, w => SetWidthKeepRight(w), targetWidth, duration)
                       .SetEase(ease)
                       .SetUpdate(true);
    }

    /// <summary>
    /// 폭을 w로 바꾸되, 오른쪽 가장자리를 고정하여 '왼쪽으로만' 확장
    /// (앵커/피벗 = 중앙(0.5,0.5) 전제)
    /// </summary>
    private void SetWidthKeepRight(float w, bool init = false)
    {
        if (init)
        {
            // 초기 한 번만 오른쪽 가장자리 좌표를 재계산하고 싶다면 여기서 갱신 가능
            // rightEdgeX = panel.anchoredPosition.x + panel.rect.width * 0.5f;
        }

        // 폭 설정
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        // 오른쪽가장자리 = rightEdgeX를 유지하기 위해 중심 x를 이동
        Vector2 pos = panel.anchoredPosition;
        pos.x = rightEdgeX - w * 0.5f;
        panel.anchoredPosition = pos;
    }
}
