using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpeedButton : MonoBehaviour
{
    public GameObject speed2X;
    public GameObject speed4X;
    public AudioSource audioSource;

    private bool IsDoubleClicked;

    float doubleClickedTime = -1.0f;
    public float interval = 0.25f; // 더블클릭 간격 (초)
    public float doubleClickActiveTime = 1f;

    void Update()
    {
        DoubleClick();

        if (IsDoubleClicked)
        {
            Debug.Log("더블클릭!");
            IsDoubleClicked = false;
            TypingManager.Instance.IsSkipDialog = true;
        }
    }

    IEnumerator ResetDoubleClick()
    {
        yield return new WaitForSeconds(doubleClickActiveTime);
        TypingManager.Instance.IsSkipDialog = false;
    }

    public void DoubleClick()
    {
        // 마우스 왼쪽 버튼(0) 클릭 또는 모바일 터치 감지
        if (Input.GetMouseButtonDown(0))
        {
            if ((Time.time - doubleClickedTime) < interval)
            {
                // --- 더블 클릭 성공 ---
                Debug.Log("Global double click!");
                IsDoubleClicked = true;
                // 더블 클릭 후 시간 초기화 (연속 클릭 방지)
                doubleClickedTime = -1.0f;

                StopCoroutine("ResetDoubleClick");

                StartCoroutine("ResetDoubleClick");
            }
            else
            {
                // --- 첫 번째 클릭 ---
                doubleClickedTime = Time.time;
            }
        }
    }

    public void ClickSpeed()
    {
        // if (TextManager.Instance.charSpeedLevel == 0)
        // {
        //     audioSource.Play();
        //     TextManager.Instance.charSpeedLevel++;
        // }
        // else if (TextManager.Instance.charSpeedLevel == 1)
        // {
        //     audioSource.Play();
        //     speed2X.SetActive(false);
        //     speed4X.SetActive(true);
        //     TextManager.Instance.charSpeedLevel++;
        // }
        // else if (TextManager.Instance.charSpeedLevel == 2)
        // {
        //     audioSource.Play();
        //     speed4X.SetActive(false);
        //     speed2X.SetActive(true);
        //     TextManager.Instance.charSpeedLevel = 0;
        // }
    }
}
