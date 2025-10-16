using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static Action healthP;
    public static Action healthM;
    public static Action mentalP;
    public static Action mentalM;
    public static Action coinP;
    public static Action coinM;
    
    public Image healthBar;
    public Image mentalBar;
    public Image coinBar;

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
    void Update()
    {
        healthBar.fillAmount = health / 3f;
        mentalBar.fillAmount = mental / 3f;
        coinBar.fillAmount = coin / 3f;
    }

    public void HealthPlus()
    {
        if (health < 3) health += 1;
        else Debug.Log("이미 풀피!");
    }
    public void HealthMinus()
    {
        if (health > 0) health -= 1;
        else Debug.Log("체력이 없습니다!!");
    }
    public void MentalPlus()
    {
        if (mental < 3) mental += 1;
        Debug.Log("풀멘탈!");
    }
    public void MentalMinus()
    {
        if (mental > 0) mental -= 1;
        else Debug.Log("멘탈 부족!");
    }
    public void CoinPlus()
    {
        if (coin < 3) coin += 1;
        else Debug.Log("이미 풀코인!");
    }
    public void CoinMinus()
    {
        if (coin > 0) coin -= 1;
        else Debug.Log("코인 부족!");
    }
}
