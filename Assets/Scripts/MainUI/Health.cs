using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Actions")]
    public static Action healthP;
    public static Action healthM;
    public static Action mentalP;
    public static Action mentalM;
    public static Action coinP;
    public static Action coinM;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip plus;
    public AudioClip minus;

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

    private void Awake()
    {
        healthP += HealthPlus;
        healthM += HealthMinus;
        mentalP += MentalPlus;
        mentalM += MentalMinus;
        coinP += CoinPlus;
        coinM += CoinMinus;
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
        if (health == 1)
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
        if (mental == 1)
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

    public void HealthPlus()
    {
        if (health < 3)
        {
            health += 1;
            audioSource.PlayOneShot(plus);

            SaveLoadManager.Instance.health = health;
            SaveLoadManager.Instance.mental = mental;
            SaveLoadManager.Instance.coin = coin;
            SaveLoadManager.Instance.SaveGameData();
        }
        else Debug.Log("이미 풀피!");
    }
    public void HealthMinus()
    {
        if (health > 0)
        {
            health -= 1;
            audioSource.PlayOneShot(minus);
        }

        if (health == 0)
    {   
        hp_1.gameObject.SetActive(false);

        Debug.Log("체력이 0입니다.");

            if (gameOverUI != null)
            {
                gameOverUI.ShowHpGameOver();
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
    public void MentalPlus()
    {
        if (mental < 3)
        {
            mental += 1;
            audioSource.PlayOneShot(plus);

            SaveLoadManager.Instance.health = health;
            SaveLoadManager.Instance.mental = mental;
            SaveLoadManager.Instance.coin = coin;
            SaveLoadManager.Instance.SaveGameData();
        }
        Debug.Log("풀멘탈!");
    }
    public void MentalMinus()
    {
        if (mental > 0)
        {
            mental -= 1;
            audioSource.PlayOneShot(minus);
        }
        if (mental == 0)
    {   
        hp_1.gameObject.SetActive(false);

        Debug.Log("체력이 0입니다.");

            if (gameOverUI != null)
            {
                gameOverUI.ShowMentalGameOver();
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

    public void CoinPlus()
    {
        if (coin < 3)
        {
            coin += 1;
            audioSource.PlayOneShot(plus);

            SaveLoadManager.Instance.health = health;
            SaveLoadManager.Instance.mental = mental;
            SaveLoadManager.Instance.coin = coin;
            SaveLoadManager.Instance.SaveGameData();
        }
        else Debug.Log("이미 풀코인!");
    }
    public void CoinMinus()
    {
        if (coin > 0)
        {
            coin -= 1;
            audioSource.PlayOneShot(minus);

            SaveLoadManager.Instance.health = health;
            SaveLoadManager.Instance.mental = mental;
            SaveLoadManager.Instance.coin = coin;
            SaveLoadManager.Instance.SaveGameData();
        }
        else Debug.Log("코인 부족!");
    }
}
