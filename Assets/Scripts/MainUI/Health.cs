using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class Health : MonoBehaviour
{
    public ScreenShake screenShake;
    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip plus;
    public AudioClip minus;
    public RectTransform hpUIRect;
    public RectTransform mtUIRect;
    public RectTransform coinUIRect;

    [Header("HP")]
    public Image hp_1;
    public Image hp_2;
    public Image hp_3;

    [Header("Mental")]
    public Image mt_1;
    public Image mt_2;
    public Image mt_3;

    [Header("Coin")]
    public Image coin_1;
    public Image coin_2;
    public Image coin_3;

    [Header("Count")]
    public int health;
    public int mental;
    public int coin;

    [Header("GameOver")]
    public GameOverUI gameOverUI;

    private enum PendingGameOverType { None, HpZero, MentalZero }
    private PendingGameOverType pendingGameOver = PendingGameOverType.None;
    private bool isWaitingGameOver = false;
    private static Health instance;
    public static Health Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No Health Instance");
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        if (GameManager.Instance.IsContinue)
        {
            health = SaveLoadManager.Instance.health;
            mental = SaveLoadManager.Instance.mental;
            coin = SaveLoadManager.Instance.coin;
        }
    }

    void Update()
    {
        HealthUpdate();
    }
    // 체력,정신력,코인 값에 따라 UI 지속적으로 업데이트
    void HealthUpdate()
    {
        // 체력 업데이트
        if (health == 0)
        {
            hp_1.gameObject.SetActive(false);
            hp_2.gameObject.SetActive(false);
            hp_3.gameObject.SetActive(false);
        }
        else if (health == 1)
        {
            hp_1.gameObject.SetActive(true);
            hp_2.gameObject.SetActive(false);
            hp_3.gameObject.SetActive(false);
        }
        else if (health == 2)
        {
            hp_1.gameObject.SetActive(true);
            hp_2.gameObject.SetActive(true);
            hp_3.gameObject.SetActive(false);
        }
        else if (health == 3)
        {
            hp_1.gameObject.SetActive(true);
            hp_2.gameObject.SetActive(true);
            hp_3.gameObject.SetActive(true);
        }

        // 정신력 업데이트
        if (mental == 0)
        {
            mt_1.gameObject.SetActive(false);
            mt_2.gameObject.SetActive(false);
            mt_3.gameObject.SetActive(false);
        }
        else if (mental == 1)
        {
            mt_1.gameObject.SetActive(true);
            mt_2.gameObject.SetActive(false);
            mt_3.gameObject.SetActive(false);
        }
        else if (mental == 2)
        {
            mt_1.gameObject.SetActive(true);
            mt_2.gameObject.SetActive(true);
            mt_3.gameObject.SetActive(false);
        }
        else if (mental == 3)
        {
            mt_1.gameObject.SetActive(true);
            mt_2.gameObject.SetActive(true);
            mt_3.gameObject.SetActive(true);
        }
        // 코인 업데이트
        if (coin == 0)
        {
            coin_1.gameObject.SetActive(false);
            coin_2.gameObject.SetActive(false);
            coin_3.gameObject.SetActive(false);
        }
        if (coin == 1)
        {
            coin_1.gameObject.SetActive(true);
            coin_2.gameObject.SetActive(false);
            coin_3.gameObject.SetActive(false);
        }
        else if (coin == 2)
        {
            coin_1.gameObject.SetActive(true);
            coin_2.gameObject.SetActive(true);
            coin_3.gameObject.SetActive(false);
        }
        else if (coin == 3)
        {
            coin_1.gameObject.SetActive(true);
            coin_2.gameObject.SetActive(true);
            coin_3.gameObject.SetActive(true);
        }
    }

    public void HealthPlus(int count)
    {
        if (health + count <= 3) health += count;
        else health = 3;

        audioSource.PlayOneShot(plus);

        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }
    public void HealthMinus(int count)
    {
        if (screenShake != null)
            screenShake.Shake(0.4f);

        //ShakeUI(hpUIRect);

        if (health - count > 0) health -= count;
        else health = 0;

        audioSource.PlayOneShot(minus);

        if (health == 0)
        {
            hp_1.gameObject.SetActive(false);

            Debug.Log("체력이 0입니다.");

            if (gameOverUI != null)
            {
                GameManager.Instance.IsZero = true;
                //RequestGameOver(PendingGameOverType.HpZero);
            }
            else
            {
                Debug.LogWarning("Health에 GameOverUI가 할당되어 있지 않습니다.");
            }
        }

        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }
    public void MentalPlus(int count)
    {

        if (mental + count <= 3) mental += count;
        else mental = 3;

        audioSource.PlayOneShot(plus);

        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }
    public void MentalMinus(int count)
    {
        if (screenShake != null)
            screenShake.Shake(0.4f);

        if (mental - count > 0) mental -= count;
        else mental = 0;

        audioSource.PlayOneShot(minus);

        if (mental == 0)
        {
            mt_1.gameObject.SetActive(false);

            Debug.Log("정신력이 0입니다.");

            if (gameOverUI != null)
            {
                GameManager.Instance.IsZero = true;
                //RequestGameOver(PendingGameOverType.MentalZero);
            }
            else
            {
                Debug.LogWarning("Mental에 GameOverUI가 할당되어 있지 않습니다.");
            }
        }
        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }

    public void CoinPlus(int count)
    {
        if (coin + count <= 3) coin += count;
        else coin = 3;

        audioSource.PlayOneShot(plus);

        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }
    public void CoinMinus(int count)
    {
        if (coin - count > 0) coin -= count;
        else coin = 0;

        audioSource.PlayOneShot(minus);

        SaveLoadManager.Instance.health = health;
        SaveLoadManager.Instance.mental = mental;
        SaveLoadManager.Instance.coin = coin;
        SaveLoadManager.Instance.SaveGameData();
    }
    private void RequestGameOver(PendingGameOverType type)
    {
        // 이미 예약돼 있으면 덮어쓰지 않게(원하면 덮어쓰도록 바꿔도 됨)
        if (pendingGameOver != PendingGameOverType.None) return;

        pendingGameOver = type;
        if(type == PendingGameOverType.HpZero)
            TextManager.Instance.SetDialogueFromChoice("D_1");
        else if(type == PendingGameOverType.MentalZero)
            TextManager.Instance.SetDialogueFromChoice("T_1");
        if (!isWaitingGameOver)
            StartCoroutine(WaitStoryEndAndShowGameOver());
    }

    private IEnumerator WaitStoryEndAndShowGameOver()
    {
        isWaitingGameOver = true;

        // 텍스트 출력이 모두 끝나서 IsStory가 false가 될 때까지 대기
        // (TextManager가 없으면 바로 진행)
        yield return new WaitUntil(() => TextManager.Instance.IsStory == false);

        // 한 프레임 넘겨서 UI/상태 정리될 시간 주기(선택)
        yield return null;

        if (gameOverUI == null)
        {
            pendingGameOver = PendingGameOverType.None;
            isWaitingGameOver = false;
            yield break;
        }

        // 기다리는 동안 값이 바뀌었을 수도 있으니 안전 체크
        if (pendingGameOver == PendingGameOverType.HpZero && health <= 0)
            gameOverUI.ShowHpGameOver();      // 여기서 Time.timeScale = 0 됨 :contentReference[oaicite:3]{index=3}
        else if (pendingGameOver == PendingGameOverType.MentalZero && mental <= 0)
            gameOverUI.ShowMentalGameOver();

        // 이후 재사용 대비
        pendingGameOver = PendingGameOverType.None;
        isWaitingGameOver = false;
    }
    private void ShakeUI(RectTransform targetRect)
    {   // 파라미터: 시간, 강도, 빈도(진동 횟수), 랜덤성
        targetRect.DOKill(true);
        targetRect.DOShakeAnchorPos(0.5f, 20f, 30, 90, false, true);
    }
}

