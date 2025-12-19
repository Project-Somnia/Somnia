using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Chapter1 : MonoBehaviour
{
    private bool IsFadeOver = false;
    public Image fadePanel;
    void Start()
    {
        StartCoroutine("FadeOut");
        StartCoroutine("WaitForSet");
    }

    IEnumerator WaitForSet()
    {
        while(!IsFadeOver)
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (!TextManager.Instance.IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (!GameManager.Instance.IsContinue) TextManager.Instance.SetDialogue("1_0");
        else TextManager.Instance.SetDialogue(SaveLoadManager.Instance.eventNumber);
    }

    IEnumerator FadeOut()
    {
        fadePanel.DOFade(0,1f);
        yield return new WaitForSeconds(1f);
        fadePanel.gameObject.SetActive(false);
        IsFadeOver = true;
    }
}
