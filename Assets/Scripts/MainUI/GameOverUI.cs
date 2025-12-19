using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 이동용(필요하면)

public class GameOverUI : MonoBehaviour
{
     [Header("GameOver Panels")]
    public GameObject hpGameOverPanel;      // 체력 0일 때 패널
    public GameObject mentalGameOverPanel;  // 멘탈 0일 때 패널

    private enum GameOverType
    {
        None,
        HpZero,
        MentalZero
    }

    private GameOverType currentType = GameOverType.None;

    void Start()
    {
        hpGameOverPanel.SetActive(false);
        mentalGameOverPanel.SetActive(false);
    }

    // ----------------- 체력 0일 때 호출 -----------------
    public void ShowHpGameOver()
    {   
        currentType = GameOverType.HpZero;

        hpGameOverPanel.SetActive(true);
        mentalGameOverPanel.SetActive(false);

        Time.timeScale = 0f; // 게임 정지 (UI만 동작)
    }

    // ----------------- 멘탈 0일 때 호출 -----------------
    public void ShowMentalGameOver()
    {
        currentType = GameOverType.MentalZero;

        mentalGameOverPanel.SetActive(true);
        hpGameOverPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    // "그냥 나가기" 버튼에 연결할 함수
    public void OnClickQuit()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsContinue = false;
        GameManager.Instance.IsCanContinue = false;
        GameManager.Instance.IsRetry = true;
        GameManager.Instance.IsZero = false;
        GameManager.Instance.IsGameOver = false;
        // 여기서 메인메뉴로 나가거나, 리트라이 씬 로드 등 원하는 동작
        SceneManager.LoadScene("Title");
    }

    // 광고를 끝까지 봐서 리워드를 받았을 때 실행될 함수
    void OnRewardSuccess()
    {
        hpGameOverPanel.SetActive(false);
        mentalGameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        switch (currentType)
        {
            case GameOverType.HpZero:
                // 체력 부활 처리
                GameManager.Instance.IsZero = false;
                GameManager.Instance.IsGameOver = false;
                GameManager.Instance.IsRebirth = true;
                Health.Instance.HealthPlus(1);
                TextManager.Instance.currentPage = 0;
                TextManager.Instance.SetDialogueFromChoice(TextManager.Instance.originNextEvent);
                break;

            case GameOverType.MentalZero:
                // 멘탈 부활 처리
                GameManager.Instance.IsZero = false;
                GameManager.Instance.IsGameOver = false;
                GameManager.Instance.IsRebirth = true;
                Health.Instance.MentalPlus(1);
                TextManager.Instance.currentPage = 0;
                TextManager.Instance.SetDialogueFromChoice(TextManager.Instance.originNextEvent);
                break;
        }

        // 필요하면 다음 광고를 위해 다시 로드
        reward.LoadRewardAd();
        currentType = GameOverType.None;
    }
}