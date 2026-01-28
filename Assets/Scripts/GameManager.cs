using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }

    public GameState gState;

    public GameObject gameLabel;
    private Text gameText;
    public PlayerMove player;
    public GameObject gameOption;


    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }
    void Start()
    {
        gState = GameState.Ready; //초기 게임 상태 = 준비

        //게임상태 UI 오브젝트에서 Text 컴포넌트 가져오기
        gameText = gameLabel.GetComponent<Text>();
        //상태 텍스트의 내용을 'Ready...'으로 변경
        gameText.text = "Ready...";
        gameText.color = new Color32(255, 185, 0, 255);
        StartCoroutine(ReadyToStart());
        //플레이어 오브젝트를 찾은 후 플레이의 PlayerMove 컴포넌트 받아오기
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
        MouseCuserOff();
    }

    void Update()
    {
        if (player.hp <= 0)
        {
            //플레이어의 애니메이션을 
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
            gameLabel.SetActive(true);
            gameText.text = "Game Over";
            gameText.color = new Color32(255, 0, 0, 255);
            // Transform button = gameText.transform.GetChild(0);
            gameOption.SetActive(true);
            MouseCuserOn();
            gState = GameState.GameOver;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !gameOption.activeSelf)
        {
            OpenOptionWindow();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameOption.activeSelf)
        {
            CloseOptionWindow();
        }
    }

    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(2f);
        gameText.text = "Go!";
        yield return new WaitForSeconds(0.5f);
        gameLabel.SetActive(false);
        gState = GameState.Run;
    }

    //옵션 화면 켜기
    public void OpenOptionWindow()
    {
        // 옵션 창을 활성화한다
        gameOption.SetActive(true);
        Time.timeScale = 0f;
        //게임상태를 일시 정지 상태로 변경
        gState = GameState.Pause;
        MouseCuserOn();

    }
    //계속하기 옵션
    public void CloseOptionWindow()
    {
        gameOption.SetActive(false);
        Time.timeScale = 1f;
        gState = GameState.Run;
        MouseCuserOff();
        
    }

    public void RestartGame()
    {
        //게임속도를 1배속으로 전환
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

    // 게임 종료 옵션
    public void QuitGame()
    {
        Application.Quit();
    }

    void MouseCuserOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void MouseCuserOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    } 
}
