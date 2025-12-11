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
        // 여기서 메인메뉴로 나가거나, 리트라이 씬 로드 등 원하는 동작
        SceneManager.LoadScene("Title");
    }
}