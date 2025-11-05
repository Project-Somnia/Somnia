using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainSetting : MonoBehaviour
{
    private bool IsMute = false;
    public AudioMixer audioMixer;
    public Image muteIcon;

    public void Mute()
    {
        if (!IsMute)
        {
            IsMute = true;
            muteIcon.gameObject.SetActive(true);
            audioMixer.SetFloat("MAIN", -80);
        }
        else
        {
            IsMute = false;
            muteIcon.gameObject.SetActive(false);
            audioMixer.SetFloat("MAIN", -20);
        }
    }
}
