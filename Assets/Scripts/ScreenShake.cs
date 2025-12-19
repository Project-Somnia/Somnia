using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform cameraTarget;          // Main Camera Transform
    [SerializeField] private RectTransform[] uiRoots;         // 각 Canvas의 UIRoot

    [Header("Params")]
    [SerializeField] private float amount = 12f;              // UI는 픽셀 단위라 값이 좀 큼
    [SerializeField] private float cameraAmount = 0.2f;       // 카메라는 월드 단위

    Vector3 camInitLocalPos;
    Vector2[] uiInitPos;

    Coroutine co;

    void Awake()
    {
        if (cameraTarget != null) camInitLocalPos = cameraTarget.localPosition;

        uiInitPos = new Vector2[uiRoots.Length];
        for (int i = 0; i < uiRoots.Length; i++)
            uiInitPos[i] = uiRoots[i].anchoredPosition;
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.K))
        Shake(0.2f);
}

    public void Shake(float time)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(ShakeRoutine(time));
    }

    IEnumerator ShakeRoutine(float time)
    {
        float t = time;

        while (t > 0f)
        {
            // (중요) 일시정지(Time.timeScale=0)에서도 흔들리게 하려면 unscaledDeltaTime
            t -= Time.unscaledDeltaTime;

            Vector2 rnd = Random.insideUnitCircle;

            // 카메라 흔들기
            if (cameraTarget != null)
            {
                cameraTarget.localPosition = camInitLocalPos;
                cameraTarget.localPosition += new Vector3(rnd.x * cameraAmount, rnd.y * cameraAmount, 0f);
            }

            // UI 흔들기
            for (int i = 0; i < uiRoots.Length; i++)
                uiRoots[i].anchoredPosition = uiInitPos[i] + rnd * amount;

            yield return null;
        }

        // 복구
        if (cameraTarget != null) cameraTarget.localPosition = camInitLocalPos;
        for (int i = 0; i < uiRoots.Length; i++)
            uiRoots[i].anchoredPosition = uiInitPos[i];

        co = null;
    }
}
