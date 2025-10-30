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

    private void Awake()
    {
        healthP += HealthPlus;
        healthM += HealthMinus;
        mentalP += MentalPlus;
        mentalM += MentalMinus;
        coinP += CoinPlus;
        coinM += CoinMinus;
    }

    public void HealthPlus()
    {
        if (health < 3)
        {
            health += 1;
            audioSource.PlayOneShot(plus);
        }

        if (health == 1) hp_1.gameObject.SetActive(true);
        if (health == 2) hp_2.gameObject.SetActive(true);
        if (health == 3) hp_3.gameObject.SetActive(true);

        else Debug.Log("이미 풀피!");
    }
    public void HealthMinus()
    {
        if (health > 0)
        {
            health -= 1;
            audioSource.PlayOneShot(minus);
        }

        if (health == 0) hp_1.gameObject.SetActive(false);
        if (health == 1) hp_2.gameObject.SetActive(false);
        if (health == 2) hp_3.gameObject.SetActive(false);
        else Debug.Log("체력이 없습니다!!");
    }
    public void MentalPlus()
    {
        if (mental < 3)
        {
            mental += 1;
            audioSource.PlayOneShot(plus);
        }

        if (mental == 1) mt_1.gameObject.SetActive(true);
        if (mental == 2) mt_2.gameObject.SetActive(true);
        if (mental == 3) mt_3.gameObject.SetActive(true);
        Debug.Log("풀멘탈!");
    }
    public void MentalMinus()
    {
        if (mental > 0)
        {
            mental -= 1;
            audioSource.PlayOneShot(minus);
        }

        if (mental == 0) mt_1.gameObject.SetActive(false);
        if (mental == 1) mt_2.gameObject.SetActive(false);
        if (mental == 2) mt_3.gameObject.SetActive(false);
        else Debug.Log("멘탈 부족!");
    }
    public void CoinPlus()
    {
        if (coin < 3)
        {
            coin += 1;
            audioSource.PlayOneShot(plus);
        }

        if (coin == 1) coin_1.gameObject.SetActive(true);
        if (coin == 2) coin_2.gameObject.SetActive(true);
        if (coin == 3) coin_3.gameObject.SetActive(true);
        else Debug.Log("이미 풀코인!");
    }
    public void CoinMinus()
    {
        if (coin > 0)
        {
            coin -= 1;
            audioSource.PlayOneShot(minus);
        }

        if (coin == 0) coin_1.gameObject.SetActive(false);
        if (coin == 1) coin_2.gameObject.SetActive(false);
        if (coin == 2) coin_3.gameObject.SetActive(false);
        else Debug.Log("코인 부족!");
    }
}
