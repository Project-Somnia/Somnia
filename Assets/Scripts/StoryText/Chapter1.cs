using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("WaitForSet");
    }

    IEnumerator WaitForSet()
    {
        while (!TextManager.Instance.IsDialogSet)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (!GameManager.Instance.IsContinue) TextManager.Instance.SetDialogue("2_1_C");
        else TextManager.Instance.SetDialogue(SaveLoadManager.Instance.eventNumber);
    }
}
