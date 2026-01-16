using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueParse : MonoBehaviour
{
    public Dictionary<string, TalkData[]> DialogueDictionary = new Dictionary<string, TalkData[]>();
    [SerializeField] List<ShowTalkData> ShowTalkDataList = new List<ShowTalkData>();

    private bool IsEntered = false;

    private string googleCsvUrl = "https://docs.google.com/spreadsheets/d/1XMHN-jTMhUGnjLoJdVjCC6levhM5KZKXHJGdw5wKP08/export?format=csv";
    public TalkData[] GetDialogue(string eventName)
    {
        if (!DialogueDictionary.ContainsKey(eventName))
        {
            TextManager.Instance.commingsoon.SetActive(true);
            Debug.Log($"대화 데이터를 찾을 수 없습니다: {eventName}");
            return null;  // 또는 빈 배열 return new TalkData[0];
        }
        else return DialogueDictionary[eventName];
    }

    private static DialogueParse instance;
    public static DialogueParse Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No TextManagerInstance");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

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
        // 윈도우/맥 줄바꿈 호환성을 위해 \r 제거 후 분리
        string[] rows = csvText.Replace("\r", "").Split('\n');

        for (int i = 1; i < rows.Length; i++)
        {
            string[] rowValues = ParseCsvLine(rows[i]);

            // 데이터가 없거나 주석(#)인 경우 스킵
            if (rowValues.Length < 1 || string.IsNullOrWhiteSpace(rowValues[0]) || rowValues[0].StartsWith("#")) continue;

            List<TalkData> talkDataList = new List<TalkData>();
            string eventName = rowValues[0].Trim();

            // 같은 이벤트 그룹(end가 나올 때까지) 처리
            while (i < rows.Length && rowValues.Length > 0 && rowValues[0].Trim() != "end")
            {
                List<string> contextList = new List<string>();
                TalkData talkData = new TalkData();

                // 기본 데이터 매핑
                talkData.selectEventNumber = rowValues[0].Trim();
                talkData.eventImage = rowValues.Length > 1 ? rowValues[1].Trim() : "";

                // 선택지 데이터가 있는 경우에만 매핑 (인덱스 에러 방지)
                if (rowValues.Length > 8)
                {
                    talkData.selectText1 = rowValues[3].Trim();
                    talkData.triggerEvent1 = rowValues[4].Trim();
                    talkData.selectText2 = rowValues[5].Trim();
                    talkData.triggerEvent2 = rowValues[6].Trim();
                    talkData.selectText3 = rowValues[7].Trim();
                    talkData.triggerEvent3 = rowValues[8].Trim();
                }

                // --- 텍스트(대사) 여러 줄 파싱 로직 시작 ---
                do
                {
                    // Trim()을 사용하여 앞뒤에 숨겨진 공백이나 유령 문자를 먼저 제거합니다.
                    string rawText = (rowValues.Length > 2) ? rowValues[2].Trim() : "";

                    // 빈칸이면
                    if(string.IsNullOrWhiteSpace(rawText))
                    {
                        rawText += "\n"; 
                        contextList.Add(rawText);
                    }
                    else
                    {
                        rawText += "\n";
                        contextList.Add(rawText);
                        IsEntered = false;
                    }

                    if (++i < rows.Length) rowValues = ParseCsvLine(rows[i]);
                    else break;

                } while (rowValues.Length > 0 && rowValues[1].Trim() != "end" && rowValues[0].Trim() != "end");

                // --- 텍스트 파싱 끝 ---
                talkData.showText = contextList.ToArray();
                talkDataList.Add(talkData);

                // 루프 조건 재확인을 위해 (do-while 탈출 후 현재 rowValues 상태가 중요)
                // 이미 위에서 i++ 하고 rowValues를 갱신했으므로 그대로 진행
            }

            if (!DialogueDictionary.ContainsKey(eventName))
            {
                DialogueDictionary.Add(eventName, talkDataList.ToArray());
            }
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
                // 이중 따옴표("") 처리: 엑셀에서 따옴표 하나를 표현할 때 사용함
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    field += '"'; // 따옴표 하나를 문자로 추가
                    i++; // 다음 따옴표 건너뜀
                }
                else
                {
                    // 문법적인 따옴표(시작과 끝)를 만났을 때
                    // field += '"'; // <--- 만약 모든 따옴표를 다 보고 싶다면 이 주석을 해제하세요.
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
