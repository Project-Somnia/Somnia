using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 이동용(필요하면)

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel; // GameOverPanel 오브젝트
    public Reward reward;            // 방금 만든 Reward 스크립트

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 정지 (UI만 동작)

        // 미리 광고를 로드해 둠
        reward.LoadRewardAd();
    }

    // "광고 보고 부활" 버튼에 연결할 함수
    public void OnClickWatchAd()
    {
        // 광고를 보고 나서 콜백으로 부활 처리
        reward.ShowRewardAd(OnRewardSuccess);
    }

    // "그냥 나가기" 버튼에 연결할 함수
    public void OnClickQuit()
    {
        Time.timeScale = 1f;
        // 여기서 메인메뉴로 나가거나, 리트라이 씬 로드 등 원하는 동작
        SceneManager.LoadScene("Title");
    }

    // 광고를 끝까지 봐서 리워드를 받았을 때 실행될 함수
    void OnRewardSuccess()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;

        // 플레이어 체력 +1
        Health.healthP();

        // 필요하면 다음 광고를 위해 다시 로드
        reward.LoadRewardAd();
    }
}