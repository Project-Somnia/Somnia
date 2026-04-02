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
    private string curNum;

    float fadeTime = 2f;
    float timeElp = 0;

    void Start()
    {
        IsPlayingTrack1 = true;
        if(GameManager.Instance.IsContinue) SetAudioTrack(SaveLoadManager.Instance.eventNumber.Split('_')[0]);
        else SetAudioTrack("1");
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
        else if(eventNumber == "R" || eventNumber == "T" || eventNumber == "D")
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
    public void CheckAudioStat(string eventNum)
    {
        curNum = TextManager.Instance.curSplitEvent;
        // 똑같으면 return
        if (curNum == eventNum) return;

        //1,2,3은 같은 음악
        if (curNum == "1" && (eventNum == "2" || eventNum == "3"))
            return;

        if (curNum == "2" && (eventNum == "1" || eventNum == "3"))
            return;

        if (curNum == "3" && (eventNum == "1" || eventNum == "2"))
            return;

        //4,5,6은 같은 음악
        if (curNum == "4" && (eventNum == "5" || eventNum == "6"))
            return;

        if (curNum == "5" && (eventNum == "4" || eventNum == "6"))
            return;

        if (curNum == "6" && (eventNum == "4" || eventNum == "5"))
            return;

        //7,8,9는 같은 음악
        if (curNum == "7" && (eventNum == "8" || eventNum == "9"))
            return;

        if (curNum == "8" && (eventNum == "7" || eventNum == "9"))
            return;

        if (curNum == "9" && (eventNum == "7" || eventNum == "8"))
            return;

        //10,11은 같은 음악
        if (curNum == "10" && eventNum == "11")
            return;

        if (curNum == "R" && (eventNum == "D" || eventNum == "T")) return;
        if (curNum == "D" && (eventNum == "R" || eventNum == "T")) return;
        if (curNum == "T" && (eventNum == "D" || eventNum == "R")) return;

        curNum = eventNum;
        TextManager.Instance.curSplitEvent = curNum;
        SetAudioTrack(eventNum);
    }


}
