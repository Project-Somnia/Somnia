using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No AudioManagerInstance");
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource track1, track2;
    public AudioClip[] audioClips = new AudioClip[5];
    private bool IsPlayingTrack1;

    float fadeTime = 2f;
    float timeElp = 0;

    void Start()
    {
        IsPlayingTrack1 = true;
        //SwapTrack(defAmbience);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            //SwapTrack(defAmbience);
        }
    }

    public void SetAudioTrack(string eventNumber)
    {
        if(eventNumber == "1" || eventNumber == "2" || eventNumber == "3") 
            SwapTrack(audioClips[0]);
        else if(eventNumber == "4" || eventNumber == "5" || eventNumber == "6") 
            SwapTrack(audioClips[1]);
        else if(eventNumber == "7" || eventNumber == "8" || eventNumber == "9") 
            SwapTrack(audioClips[2]);
        else if(eventNumber == "10" || eventNumber == "11")
            SwapTrack(audioClips[3]);
        else if(eventNumber == "R")
            SwapTrack(audioClips[4]);
        else
            Debug.LogError("해당 이벤트에 적용되는 오디오가 없습니다!");
    }

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip));

        IsPlayingTrack1 = !IsPlayingTrack1;
    }

    IEnumerator FadeTrack(AudioClip newClip)
    {
        if (IsPlayingTrack1)
        {
            track2.clip = newClip;
            track2.Play();

            while (timeElp < fadeTime)
            {
                track2.volume = Mathf.Lerp(0, 1, timeElp / fadeTime);
                track1.volume = Mathf.Lerp(1, 0, timeElp / fadeTime);
                timeElp += Time.deltaTime;
                yield return null;
            }
            track1.Stop();
        }
        else
        {
            track1.clip = newClip;
            track1.Play();

            while (timeElp < fadeTime)
            {
                track1.volume = Mathf.Lerp(0, 1, timeElp / fadeTime);
                track2.volume = Mathf.Lerp(1, 0, timeElp / fadeTime);
                timeElp += Time.deltaTime;
                yield return null;
            }
            track2.Stop();
        }
        timeElp = 0;
    }


}
