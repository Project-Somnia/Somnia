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
    public GameObject nextButtons;

    private bool IsFade = false;
    private bool IsCanStart = false;
    private bool IsHalf = false;

    private float fadeTime = 3f;

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
        Debug.Log("나여");

        fadeImg.gameObject.SetActive(true);
        Color fadeCol = fadeImg.color; // 컬러 따오기

        Tween t = fadeImg.DOFade(1, fadeTime);
        yield return new WaitForSeconds(fadeTime / 2);
        if (!IsHalf && fadeImg.gameObject.CompareTag("BG"))
        {
            IsHalf = true;
            StartCoroutine(FadeIn(bg3));
            StartCoroutine(FadeIn(startButton));
            StartCoroutine(FadeIn(startText));
        }
        yield return new WaitForSeconds(fadeTime / 2);

        if (fadeImg.gameObject.CompareTag("StartUI"))
        {
            UpDownButton();
            //IsCanStart = true;
        }
        else if (fadeImg.gameObject.CompareTag("StartText"))
        {
            StartCoroutine(BlinkOut(startText));
        }
        t.Kill();
    }
    IEnumerator FadeOut(Image fadeImg)
    {
        Color fadeCol = fadeImg.color; // 컬러 따오기
        fadeImg.DOFade(0f, fadeTime);

        yield return new WaitForSeconds(fadeTime);
        fadeImg.gameObject.SetActive(false); // 이미지 끄기
    }

    IEnumerator BlinkIn(Image blinkImg)
    {
        blinkImg.gameObject.SetActive(true);
        Color blinkCol = blinkImg.color; // 컬러 따오기
        Tween t = blinkImg.DOFade(1f, fadeTime);
        t.Kill();

        yield return new WaitForSeconds(fadeTime);
        if(!IsCanStart)
        StartCoroutine(BlinkOut(blinkImg));
    }

    IEnumerator BlinkOut(Image blinkImg)
    {
        Color blinkCol = blinkImg.color; // 컬러 따오기
        Tween t = blinkImg.DOFade(0.3f, fadeTime);
        t.Kill();

        yield return new WaitForSeconds(fadeTime);
        blinkImg.gameObject.SetActive(false); // 이미지 끄기
        if(!IsCanStart)
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

    public void NextButton()
    {
        startButton.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        nextButtons.SetActive(true);
        IsCanStart = true;
    }
    public void StartGame()
    {
        if (IsCanStart) SceneManager.LoadScene("Game");
    }
}
