using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// 로딩 연출
// 전달받은 씬으로 이동

public class SceneLoading : MonoBehaviour
{
    private static string targetScene;  // 이동할 씬 이름
    
    [SerializeField] private Slider loadingGauge;   // 인스펙터에 로딩바 넣기
    [SerializeField] private TMP_Text loadingText;      // 인스펙터에 퍼센트 텍스트 넣기

    private float loadingTime = 1f;     // 로딩 연출 대기 시간 / 로딩시간이 상당히 짧아서 대기 1초 넣음

    // 외부에서 호출
    public static void LoadTo(string sceneName)
    { 
        targetScene = sceneName;

        // 로딩씬으로 이동
        SceneManager.LoadScene("Scene Loading");
    }

    void Start()
    {
        loadingGauge.value = 0f; // 로딩바 초기화
        loadingText.text = "0%";
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(targetScene);
        ao.allowSceneActivation = false;

        float current = 0f; // 로딩 게이지 초기값

        while (!ao.isDone)
        {
            // 1. 로딩 게이지 세팅
            float target = Mathf.Clamp01(ao.progress / 0.9f);
            current = Mathf.MoveTowards(current, target, Time.deltaTime);
            loadingGauge.value = current;

            // 2. 로딩 퍼센트 세팅
            int pecent = Mathf.RoundToInt(current * 100f);
            loadingText.text = pecent + "%";

            if (current >= 1f)
            {
                // 로딩 연출중 딜레이
                yield return new WaitForSeconds(loadingTime);
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
