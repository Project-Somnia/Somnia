using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueParse : MonoBehaviour
{

    public static Dictionary<string, TalkData[]> DialogueDictionary = new Dictionary<string, TalkData[]>();
    [SerializeField] public TextAsset csvFile = null;
    [SerializeField] List<ShowTalkData> ShowTalkDataList = new List<ShowTalkData>();

    void SetShowTalkData()
    {
        // 딕셔너리의 키 값들을 가진 리스트
        List<string> eventNames =
                    new List<string>(DialogueDictionary.Keys);
        // 딕셔너리의 밸류 값들을 가진 리스트
        List<TalkData[]> talkDatasList =
                    new List<TalkData[]>(DialogueDictionary.Values);

        // 딕셔너리의 크기만큼 추가
        for (int i = 0; i < eventNames.Count; i++)
        {
            ShowTalkData showTalk =
                new ShowTalkData(eventNames[i], talkDatasList[i]);

            ShowTalkDataList.Add(showTalk);
        }
    }

    public static TalkData[] GetDialogue(string eventName)
    {
        return DialogueDictionary[eventName];
    }

    private void Awake()
    {
        SetTalkDictionary();
        SetShowTalkData();
    }

    public void SetTalkDictionary()
    {
        // 줄바꿈 기준으로 csv 줄 분할
        string[] rows = csvFile.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < rows.Length; i++)
        {
            string[] rowValues = ParseCsvLine(rows[i]);

            // 유효하지 않은 이벤트 이름이면 건너뜀
            if (rowValues[0].Trim() == "" || rowValues[0].Trim() == "end") continue;

            Debug.Log("처음: " + rowValues[0].Trim());

            List<TalkData> talkDataList = new List<TalkData>();
            string eventName = rowValues[0].Trim(); // A열: 이벤트 이름

            while (rowValues[0].Trim() != "end")
            {
                List<string> contextList = new List<string>();
                TalkData talkData;

                talkData.name = rowValues[1].Trim();    // B열: 캐릭터 이름
                talkData.emotionState = rowValues.Length > 3 ? rowValues[3].Trim() : ""; // D열: 감정 상태

                Debug.Log("이름 체크: " + talkData.name);

                // 같은 화자의 연속 대사 처리
                do
                {
                    if (rowValues.Length > 2)
                        contextList.Add(rowValues[2].Trim());

                    if (++i < rows.Length)
                        rowValues = ParseCsvLine(rows[i]);
                    else
                        break;

                } while (rowValues[1].Trim() == "" && rowValues[0].Trim() != "end");

                talkData.contexts = contextList.ToArray();
                talkDataList.Add(talkData);
            }

            DialogueDictionary.Add(eventName, talkDataList.ToArray());
        }
    }

    // 쉼표와 큰따옴표 처리용 CSV 파서
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
                inQuotes = !inQuotes; // 따옴표 상태 전환
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(field);
                field = "";
            }
            else
            {
                field += c;
            }
        }

        result.Add(field); // 마지막 필드 추가
        return result.ToArray();
    }
}
