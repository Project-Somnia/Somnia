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
        Debug.Log(healthBar.fillAmount);
    }

    public void HealthPlus()
    {
        health += 1;
    }
    public void HealthMinus()
    {
        health -= 1;
    }
    public void MentalPlus()
    {
        mental += 1;
    }
    public void MentalMinus()
    {
        mental -= 1;
    }
    public void CoinPlus()
    {
        coin += 1;
    }
    public void CoinMinus()
    {
        coin -= 1;
    }
}
