using System.Collections.Generic;
using UnityEngine;

public class EncounterFlowManager : MonoBehaviour
{   
    [Range(0f, 1f)]
    public float randomEncounterRate = 0.1f; // 0.3 = 30% 확률

    // stage 번호 (1,2,3...) 별 랜덤 인카운터 리스트
    private List<string> randomEncounterCandidates = new List<string>();

    private bool isBuilt = false;

    private static EncounterFlowManager instance;
    public static EncounterFlowManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No EncounterFlowManagerInstance");
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

        // DontDestroyOnLoad(gameObject);
    }

    /// 현재 이벤트와 CSV에 적힌 triggerevent를 받아서,
    /// "랜덤 인카운터를 낄지 말지"를 결정하고 최종 eventId를 돌려준다.
    public string DecideNextEvent(string currentEventId, string triggerEventId)
    {
        if (string.IsNullOrEmpty(triggerEventId))
            return null;

        BuildRandomTableIfNeeded();

        // 지금 이벤트가 이미 랜덤 인카운터라면, 더 이상 랜덤은 굴리지 않고
        // CSV에 적힌 목적지로 바로 간다.
        if (IsRandomEncounter(currentEventId))
        {
            return triggerEventId;
        }

        int curStage = GetStageFromEventId(currentEventId);
        int nextStage = GetStageFromEventId(triggerEventId);

        bool isCrossStage = (curStage != 0 && nextStage != 0 && curStage != nextStage);

        // 1_x -> 2_0, 2_x -> 3_0 처럼 "챕터가 바뀌는 시점"에만 랜덤 체크
        if (isCrossStage)
        {
            if (UnityEngine.Random.value < randomEncounterRate)
            {   
                Debug.Log($"{randomEncounterRate}");
                string randomId = GetRandomEncounterGlobal();
                if (!string.IsNullOrEmpty(randomId))
                {
                    TextManager.Instance.pendingMainEventId = triggerEventId;
                    Debug.Log($"[Encounter] {currentEventId} -> {randomId} (원래 목적지: {triggerEventId})");
                    return randomId;
                }
            }
        }

        // 랜덤 실패하거나 랜덤 풀 없으면 그냥 원래 triggerevent로 진행
        return triggerEventId;
    }

    #region Random Table

    void BuildRandomTableIfNeeded()
    {
        if (isBuilt) return;

        randomEncounterCandidates.Clear();

        // DialogueParse.DialogueDictionary: CSV 파싱이 끝나면 eventName -> TalkData[] 저장됨
        foreach (var kvp in DialogueParse.DialogueDictionary)
        {
            string eventId = kvp.Key; // "1_0", "2_0", "R_1_0" 등
            
            //랜덤 인카운터 발생했는지 체크, continue는 list.Add(eventId)를 하지말고 건너뛰어라 라는 의미
            if (!IsRandomEncounter(eventId))
                continue;
            
            //eventId가 0으로 끝나는지 (랜덤 인카운터의 진입 이벤트인지) 체크
            if (!IsMainRandomCandidate(eventId))
                continue;

            randomEncounterCandidates.Add(eventId);

            Debug.Log($"[EncounterFlowManager] 랜덤 후보 개수: {randomEncounterCandidates.Count}");
        }
        //buildRandomTable은 한번만 만들어지면 됌.
        isBuilt = true;
    }

    bool IsRandomEncounter(string eventId)
    {
        return !string.IsNullOrEmpty(eventId) && eventId.StartsWith("R_");
    }

    /// <summary>
    /// "1_0" -> 1, "2_3" -> 2, "R_1_0" -> 1, "R_2_1" -> 2
    /// 실패하면 0 리턴
    /// </summary>
    int GetStageFromEventId(string eventId)
    {
        if (string.IsNullOrEmpty(eventId))
            return 0;

        string[] parts = eventId.Split('_');

        try
        {
            if (eventId.StartsWith("R_"))
            {
                // R_1_0 -> ["R", "1", "0"] 라고 가정
                if (parts.Length > 1)
                    return int.Parse(parts[1]);
            }
            else
            {
                // 1_0 -> ["1", "0"]
                if (parts.Length > 0)
                    return int.Parse(parts[0]);
            }
        }
        catch
        {
            Debug.LogWarning($"GetStageFromEventId 파싱 실패: {eventId}");
        }

        return 0;
    }

    //랜덤 인카운터 발생시 랜덤 인카운터 후보 중 하나가 반환됌.
    string GetRandomEncounterGlobal()
    {
        if (randomEncounterCandidates == null || randomEncounterCandidates.Count == 0)
            return null;

        int idx = Random.Range(0, randomEncounterCandidates.Count);
        return randomEncounterCandidates[idx];
    }

    #endregion

    bool IsMainRandomCandidate(string eventId)
    {
        if (string.IsNullOrEmpty(eventId))
            return false;

        // 예: R_1_0, R_2_0
        string[] parts = eventId.Split('_');
        if (parts.Length < 3)
            return false;

        string last = parts[parts.Length - 1]; // "0", "A", "B" 등
        return last == "0";                    // 마지막 토큰이 "0"인 경우만 랜덤 후보
    }
}
