using System.Collections.Generic;
using UnityEngine;

public class EncounterFlowManager : MonoBehaviour
{
    public static EncounterFlowManager Instance { get; private set; }

    [Range(0f, 1f)]
    public float randomEncounterRate = 0.3f; // 0.3 = 30% 확률

    // stage 번호 (1,2,3...) 별 랜덤 인카운터 리스트
    private Dictionary<int, List<string>> randomEncounterByStage = new Dictionary<int, List<string>>();

    private bool isBuilt = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 필요하면 DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 현재 이벤트와 CSV에 적힌 triggerevent를 받아서,
    /// "랜덤 인카운터를 낄지 말지"를 결정하고 최종 eventId를 돌려준다.
    /// </summary>
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
                string randomId = GetRandomEncounterForStage(curStage);
                if (!string.IsNullOrEmpty(randomId))
                {
                    Debug.Log($"랜덤 인카운터 발생! {currentEventId} -> {randomId}");
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

        randomEncounterByStage.Clear();

        // DialogueParse.DialogueDictionary: CSV 파싱이 끝나면 eventName -> TalkData[] 저장됨
        foreach (var kvp in DialogueParse.DialogueDictionary)
        {
            string eventId = kvp.Key; // "1_0", "2_0", "R_1_0" 등

            if (!IsRandomEncounter(eventId))
                continue;

            int stage = GetStageFromEventId(eventId);
            if (stage == 0)
                continue;

            if (!randomEncounterByStage.TryGetValue(stage, out var list))
            {
                list = new List<string>();
                randomEncounterByStage.Add(stage, list);
            }

            list.Add(eventId);
        }

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

    string GetRandomEncounterForStage(int stage)
    {
        if (stage == 0) return null;

        if (!randomEncounterByStage.TryGetValue(stage, out var list) || list.Count == 0)
            return null;

        int idx = Random.Range(0, list.Count);
        return list[idx];
    }

    #endregion
}
