using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingNextScene : MonoBehaviour
{
    public int sceneNumber = 2;
    public Slider loadingBar;
    public Text loadingText;

    void Start()
    {
        // 비동기 씬 로드 코루틴을 실행한다
        StartCoroutine(TransitionNextScene(sceneNumber));
    }

    void Update()
    {

    }

    IEnumerator TransitionNextScene(int num)
{
    AsyncOperation ao = SceneManager.LoadSceneAsync(num);
    ao.allowSceneActivation = false;

    while (!ao.isDone)
    {
        float progress = Mathf.Clamp01(ao.progress / 0.9f); // 0 ~ 1로 보정
        loadingBar.value = progress;
        loadingText.text = (progress * 100f).ToString("F0") + "%";

        // 90% 이상 로딩되었으면 씬 활성화
        if (ao.progress >= 0.9f)
        {
            loadingText.text = "100%";
            loadingBar.value = 1f;

            // 약간의 딜레이를 주거나, 유저 입력 기다리기 등 가능
            yield return new WaitForSeconds(0.5f);
            ao.allowSceneActivation = true;
        }

        yield return null;
    }
}

}
