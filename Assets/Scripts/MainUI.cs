using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainUI : MonoBehaviour
{
    public Image bg;
    public Image bg2;
    public Image bg3;
    public Image startButton;
    public Image startText;
    private bool IsFade = false;
    private bool IsCanStart = false;

    public RectTransform target;  // 움직일 UI 오브젝트
    public float moveAmount = 10f; // 위아래로 이동할 거리
    public float duration = 0.5f;  // 한 번 이동하는 데 걸리는 시간

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsFade)
        {
            IsFade = true;
            StartCoroutine(FadeOut(bg));
            StartCoroutine(FadeIn(bg2));
        }
    }
    IEnumerator FadeIn(Image fadeImg)
    {
        bool IsHalf = false;
        fadeImg.gameObject.SetActive(true);
        Color fadeCol = fadeImg.color; // 컬러 따오기
        while (fadeCol.a < 1f) // 완전이 불투명해질때까지
        {
            fadeCol.a += 0.005f; // 조금씩 불투명해짐
            fadeImg.color = fadeCol; //색 덮어씌우기
            if (fadeCol.a >= 0.5f && !IsHalf && fadeImg.gameObject.CompareTag("BG"))
            {
                IsHalf = true;
                StartCoroutine(FadeIn(bg3));
                StartCoroutine(FadeIn(startButton));
                StartCoroutine(FadeIn(startText));
            }
            yield return new WaitForSeconds(0.01f); // 0.01초간 대기
        }
        if (fadeImg.gameObject.CompareTag("StartUI"))
        {
            UpDownButton();
            IsCanStart = true;
        }
        else if (fadeImg.gameObject.CompareTag("StartText"))
        {
            StartCoroutine(BlinkOut(startText));
        }
    }
    IEnumerator FadeOut(Image fadeImg)
    {
        Color fadeCol = fadeImg.color; // 컬러 따오기
        while (fadeCol.a > 0f) // 완전이 투명해질때까지
        {
            fadeCol.a -= 0.005f; // 조금씩 투명해짐
            fadeImg.color = fadeCol; //색 덮어씌우기
            yield return new WaitForSeconds(0.01f); // 0.01초간 대기
        }
        fadeImg.gameObject.SetActive(false); // 이미지 끄기
    }

    IEnumerator BlinkIn(Image blinkImg)
    {
        blinkImg.gameObject.SetActive(true);
        Color blinkCol = blinkImg.color; // 컬러 따오기
        while (blinkCol.a < 1f) // 완전이 투명해질때까지
        {
            blinkCol.a += 0.03f; // 조금씩 투명해짐
            blinkImg.color = blinkCol; //색 덮어씌우기
            yield return new WaitForSeconds(0.01f); // 0.01초간 대기
        }
        StartCoroutine(BlinkOut(blinkImg));
    }

    IEnumerator BlinkOut(Image blinkImg)
    {
        Color blinkCol = blinkImg.color; // 컬러 따오기
        while (blinkCol.a > 0.3f) // 완전이 투명해질때까지
        {
            blinkCol.a -= 0.03f; // 조금씩 투명해짐
            blinkImg.color = blinkCol; //색 덮어씌우기
            yield return new WaitForSeconds(0.01f); // 0.01초간 대기
        }
        blinkImg.gameObject.SetActive(false); // 이미지 끄기
        StartCoroutine(BlinkIn(blinkImg));
    }

    void UpDownButton()
    {
        Vector3 startPos = target.localPosition;

        // 위아래 반복 이동
        target.DOLocalMoveY(startPos.y + moveAmount, duration)
              .SetEase(Ease.InOutSine)      // 부드럽게
              .SetLoops(-1, LoopType.Yoyo); // 무한 반복, 왕복
    }

    public void StartGame()
    {
        if (IsCanStart) SceneManager.LoadScene("Game");
    }
}
