using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PaintManager : MonoBehaviour
{

    private bool IsPaintEmpty = false;
    private bool IsPaintDeath = false;
    private bool IsPreventFadeDup = false;


    [Header("Paint Sets")]
    public Image oldPaint;
    public Image newPaint;
    public Sprite[] paintSprites = new Sprite[10];
    public Sprite empty;
    public Sprite death;
    private float fadeTime = 2f;
    private int paintIdx = 0;
    private int fadeCnt = 0;
    private string currentPaint = "";
    private string paintNum = "";
    // Start is called before the first frame update
    private static PaintManager instance;
    public static PaintManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No PaintManagerInstance");
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        if (GameManager.Instance.IsContinue)
        {
            currentPaint = SaveLoadManager.Instance.paintName;
            if (currentPaint == "Empty") oldPaint.sprite = empty;
            else
            {
                paintIdx = Array.FindIndex(paintSprites, x => currentPaint.Contains(x.name));
                oldPaint.sprite = paintSprites[paintIdx];
            }
        }
        oldPaint.gameObject.SetActive(true);
        //oldPaint.DOFade(1f, fadeTime);
    }

    public void SetPaint(string paintName)
    {
        paintName = paintName.Replace(".png", "");
        paintNum = paintName.Split("_")[0];

        if (paintName == "Empty" || paintName == "") IsPaintEmpty = true;

        else if (paintName == "Death") IsPaintDeath = true;

        if (currentPaint == "" || currentPaint == "Empty" || currentPaint == "Death" || currentPaint != paintName)
        {
            currentPaint = paintName;
            SaveLoadManager.Instance.paintName = paintName;
            SaveLoadManager.Instance.SaveGameData();

            paintIdx = Array.FindIndex(paintSprites, x => currentPaint.Contains(x.name));

            FadePaint(oldPaint, newPaint);
        }
    }

    public void FadePaint(Image curPaint, Image nextPaint)
    {
        curPaint.gameObject.SetActive(true);
        nextPaint.gameObject.SetActive(true);
        // 짝수
        if (fadeCnt % 2 == 0)
        {
            if (IsPaintEmpty)
            {
                IsPaintEmpty = false;
                nextPaint.sprite = empty;
            }
            else if (IsPaintDeath)
            {
                IsPaintDeath = false;
                nextPaint.sprite = death;
            }
            else nextPaint.sprite = paintSprites[paintIdx];

            if (IsPreventFadeDup)
            {
                Color curCol = curPaint.color;
                curCol.a = 1f;
                curPaint.color = curCol;
            }

            Color nextCol = nextPaint.color;
            nextCol.a = 0f;
            nextPaint.color = nextCol;

            curPaint.DOFade(0f, fadeTime);
            nextPaint.DOFade(1f, fadeTime);
        }
        else
        {
            if (IsPaintEmpty)
            {
                IsPaintEmpty = false;
                curPaint.sprite = empty;
            }
            else if (IsPaintDeath)
            {
                IsPaintDeath = false;
                curPaint.sprite = death;
            }
            else curPaint.sprite = paintSprites[paintIdx];

            Color curCol = curPaint.color;
            curCol.a = 0f;
            curPaint.color = curCol;

            Color nextCol = nextPaint.color;
            nextCol.a = 1f;
            nextPaint.color = nextCol;

            curPaint.DOFade(1f, fadeTime);
            nextPaint.DOFade(0f, fadeTime);
        }
        IsPreventFadeDup = true;
        fadeCnt++;
    }
}
