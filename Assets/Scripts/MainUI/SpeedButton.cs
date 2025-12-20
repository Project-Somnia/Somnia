using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpeedButton : MonoBehaviour
{
    public GameObject speed2X;
    public GameObject speed4X;
    public AudioSource audioSource;
    public void ClickSpeed()
    {
        if(TextManager.Instance.charSpeedLevel == 0)
        {
            audioSource.Play();
            TextManager.Instance.charSpeedLevel++;
        }
        else if(TextManager.Instance.charSpeedLevel == 1)
        {
            audioSource.Play();
            speed2X.SetActive(false);
            speed4X.SetActive(true);
            TextManager.Instance.charSpeedLevel++;
        } 
        else if(TextManager.Instance.charSpeedLevel == 2)
        {
            audioSource.Play();
            speed4X.SetActive(false);
            speed2X.SetActive(true);
            TextManager.Instance.charSpeedLevel = 0;
        } 
    }
}
