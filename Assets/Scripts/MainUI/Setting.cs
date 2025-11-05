using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider ostSlider;
    [SerializeField] private Slider sfxSlider;

    private float mainVol;
    private float ostVol;
    private float sfxVol;
    private bool IsSetting = false;
    public GameObject esc;
    public GameObject sound;


    public void Start()
    {
        mainSlider.value = 0.5f;
        ostSlider.value = 0.5f;
        sfxSlider.value = 0.5f;

        mainVol = mainSlider.value;
        ostVol = ostSlider.value;
        sfxVol = sfxSlider.value;

        audioMixer.SetFloat("MAIN", Mathf.Log10(mainVol) * 20);
        audioMixer.SetFloat("OST", Mathf.Log10(ostVol) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void SetVolume()
    {
        mainVol = mainSlider.value;
        ostVol = ostSlider.value;
        sfxVol = sfxSlider.value;

        if (mainVol == 0)
        {
            audioMixer.SetFloat("MAIN", -80);
        }
        else
        {
            audioMixer.SetFloat("MAIN", Mathf.Log10(mainVol) * 20);
        }

        if (ostVol == 0)
        {
            audioMixer.SetFloat("OST", -80);
        }
        else
        {
            audioMixer.SetFloat("OST", Mathf.Log10(ostVol) * 20);
        }

        if (mainVol == 0)
        {
            audioMixer.SetFloat("SFX", -80);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsSetting = !IsSetting;
            esc.SetActive(IsSetting);
            sound.SetActive(IsSetting);
        }
    }
}
