using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainUI : MonoBehaviour
{
    [Header("Backgrounds")]
    public Image bg_1;
    public Image bg_2;
    public Image bg_3;

    [Header("Buttons")]
    public Image tapButton;
    public Image tapText;
    public Image newGameButton;
    public Image fadePanel;
    public TextMeshProUGUI newGameText;
    public Image continueButton;
    public TextMeshProUGUI continueText;

    public GameObject nextButtons;

    private bool IsFade = false;
    private bool IsCanStart = false;
    private bool IsHalf = false;
    private bool IsFadeToGame = false;

    private float fadeTime = 2.5f;

    public RectTransform target;  // 움직일 UI 오브젝트
    public float moveAmount = 10f; // 위아래로 이동할 거리
    public float duration = 0.5f;  // 한 번 이동하는 데 걸리는 시간

    void Start()
    {
        SaveLoadManager.Instance.LoadGameData();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsFade)
        {
            IsFade = true;
            StartCoroutine(FadeOut(bg_1));
            StartCoroutine(FadeIn(bg_2));
        }
    }
    IEnumerator FadeIn(Image fadeImg)
    {
        fadeImg.gameObject.SetActive(true);
        Tween t = fadeImg.DOFade(1, fadeTime);
        yield return new WaitForSeconds(fadeTime / 2);

        if (!IsHalf && fadeImg.gameObject.CompareTag("BG"))
        {
            IsHalf = true;
            StartCoroutine(FadeIn(bg_3));
            StartCoroutine(FadeIn(newGameButton));

            if (GameManager.Instance.IsCanContinue) StartCoroutine(FadeIn(continueButton));

            t = newGameText.DOFade(1, fadeTime);
            t = continueText.DOFade(1, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime / 2);

        if (fadeImg.gameObject.CompareTag("TapTitle"))
        {
            IsCanStart = true;
            StartCoroutine(BlinkOut(fadeImg));
        }
        t.Kill();
    }
    IEnumerator FadeOut(Image fadeImg)
    {
        fadeImg.DOFade(0f, fadeTime);

        yield return new WaitForSeconds(fadeTime);
        fadeImg.gameObject.SetActive(false); // 이미지 끄기
    }

    IEnumerator BlinkIn(Image blinkImg)
    {
        blinkImg.gameObject.SetActive(true);
        Color blinkCol = blinkImg.color; // 컬러 따오기
        Tween t = blinkImg.DOFade(1f, fadeTime);

        yield return new WaitForSeconds(fadeTime);
        t.Kill();
        StartCoroutine(BlinkOut(blinkImg));
    }

    IEnumerator BlinkOut(Image blinkImg)
    {
        Color blinkCol = blinkImg.color; // 컬러 따오기
        Tween t = blinkImg.DOFade(0.3f, fadeTime);

        yield return new WaitForSeconds(fadeTime);

        t.Kill();
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

    public void NextButton()
    {
        tapButton.gameObject.SetActive(true);
        tapText.gameObject.SetActive(true);

        StartCoroutine(FadeIn(tapButton));
        StartCoroutine(FadeIn(tapText));
        UpDownButton();

        nextButtons.SetActive(false);
    }
    public void StartGame()
    {
        if (IsCanStart)
        {
            Debug.Log("시작!");
            GameManager.Instance.IsRetry = false;
            GameManager.Instance.IsContinue = false;
            StopAllCoroutines();
            if (!IsFadeToGame)
            {
                IsFadeToGame = true;
                StartCoroutine("FadeToGame");
            }

            //SceneManager.LoadScene("GameKabocha2");
        }
    }
    public void ContinueGame()
    {
        SaveLoadManager.Instance.LoadGameData();
        GameManager.Instance.IsContinue = true;
        StopAllCoroutines();
        if (!IsFadeToGame)
        {
            IsFadeToGame = true;
            StartCoroutine("FadeToGame");
        }
        //SceneManager.LoadScene("GameKabocha2");
    }

    IEnumerator FadeToGame()
    {
        fadePanel.gameObject.SetActive(true);   
        fadePanel.DOFade(1,fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("GameKabocha2");
    }
}
