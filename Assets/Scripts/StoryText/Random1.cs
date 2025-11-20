using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random1 : MonoBehaviour
{
    public static bool isChapterEnd = false;
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
        TextManager.Instance.SetDialogue("R_1_0");
    }
}
