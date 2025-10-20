using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueParse : MonoBehaviour
{
    public static Dictionary<string, TalkData[]> DialogueDictionary = new Dictionary<string, TalkData[]>();
    [SerializeField] List<ShowTalkData> ShowTalkDataList = new List<ShowTalkData>();

    private string googleCsvUrl = "https://docs.google.com/spreadsheets/d/1XMHN-jTMhUGnjLoJdVjCC6levhM5KZKXHJGdw5wKP08/export?format=csv";
    public static TalkData[] GetDialogue(string eventName)
    {
        return DialogueDictionary[eventName];
    }
    private void Awake()
    {
        StartCoroutine(DownloadAndParseCSV());
    }

    IEnumerator DownloadAndParseCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(googleCsvUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("CSV 다운로드 실패: " + www.error);
        }
        else
        {
            string csvText = www.downloadHandler.text;
            SetTalkDictionary(csvText);
            SetShowTalkData();
        }
    }

    public void SetTalkDictionary(string csvText)
    {
        string[] rows = csvText.Split(new char[] { '\n' });

        for (int i = 1; i < rows.Length; i++)
        {
            string[] rowValues = ParseCsvLine(rows[i]);

            if (rowValues[0].Trim() == "" || rowValues[0].Contains("#")) continue;

            List<TalkData> talkDataList = new List<TalkData>();
            string eventName = rowValues[0].Trim();

            while (rowValues[0].Trim() != "end")
            {
                List<string> contextList = new List<string>();
                TalkData talkData = new TalkData();

                talkData.selectEventNumber = rowValues[0].Trim();
                talkData.eventImage = rowValues[1].Trim();
                talkData.selectText1 = rowValues[3].Trim();
                talkData.triggerEvent1 = rowValues[4].Trim();
                talkData.selectText2 = rowValues[5].Trim();
                talkData.triggerEvent2 = rowValues[6].Trim();
                talkData.selectText3 = rowValues[7].Trim();
                talkData.triggerEvent3 = rowValues[8].Trim();
                do
                {
                    // contextList.Add(rowValues[2].Trim());
                    contextList.Add(rowValues[2].Trim('"', '\r', '\n'));

                    if (++i < rows.Length)
                        rowValues = ParseCsvLine(rows[i]);
                    else
                        break;

                } while (rowValues[1].Trim() == "" && rowValues[0].Trim() != "end");

                if (GameManager.Instance.IsSelectEvent1)
                {
                    contextList.Add(talkData.selectText1);
                }

                talkData.showText = contextList.ToArray();
                talkDataList.Add(talkData);
            }

            DialogueDictionary.Add(eventName, talkDataList.ToArray());
            
        }
    }

    void SetShowTalkData()
    {
        List<string> eventNames = new List<string>(DialogueDictionary.Keys);
        List<TalkData[]> talkDatasList = new List<TalkData[]>(DialogueDictionary.Values);

        for (int i = 0; i < eventNames.Count; i++)
        {
            ShowTalkData showTalk = new ShowTalkData(eventNames[i], talkDatasList[i]);
            ShowTalkDataList.Add(showTalk);
        }
        TextManager.Instance.IsDialogSet = true;
    }

    // 쉼표 & 큰따옴표 파서
    private string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string field = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // 이중 따옴표 처리
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    field += '"';
                    i++; // skip one more
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(field.Trim());
                field = "";
            }
            else
            {
                field += c;
            }
        }

        result.Add(field.Trim());
        return result.ToArray();
    }
}
