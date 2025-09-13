using UnityEngine;

[System.Serializable] // 구조체가 인스펙터 창에 보이게 하기 위한 작업
public struct TalkData
{
    public string selectEventNumber; // 이벤트 번호
    public string eventImage; // 이미지 파일 이름
    public string[] showText; // 메인 대사
    public string selectText1; // 순차적 선택지1 대사
    public string triggerEvent1; // 랜덤 이벤트1 대사
    public string selectText2; // 순차적 선택지2 대사
    public string triggerEvent2; // 랜덤 이벤트2 대사
    public string selectText3; // 순차적 선택지3 대사
    public string triggerEvent3; // 랜덤 이벤트3 대사
}

[System.Serializable]
public class ShowTalkData
{
    public string eventName;
    public TalkData[] talkDatas;
    
    public ShowTalkData(string name, TalkData[] td)
    {
        eventName = name;
        talkDatas = td;
    }
}

public class Dialogue : MonoBehaviour
{
    // 위에서 선언한 TalkData 배열 
    [SerializeField] TalkData[] talkDatas;

    public TalkData[] GetObjectDialogue()
    {
        return DialogueParse.GetDialogue(TextManager.Instance.storyEventName);
    }
}