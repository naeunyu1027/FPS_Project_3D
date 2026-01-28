    using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{

    public InputField id;
    public InputField password;

    //검사 텍스트 변수
    public Text notify;
    void Start()
    {
        notify.text = "";
    }

    void Update()
    {

    }

    public void SaveUserData()
    {   //만일 시스템에 저장되어 있는 아이디가 존재하지 않는다면
        if (!PlayerPrefs.HasKey(id.text))
        {
            //사용자의 아이디는 키(key)로 패스워드는 값(value)로 설정해 저장
            PlayerPrefs.SetString(id.text, password.text);
            notify.text = "아이디 생성이 완료되었습니다";
        }
        else
        {
            notify.text = "이미 존재하는 아이디 입니다.";
        }
    }

    //로그인 메서드
    public void CheckUserData()
    {
        //사용자가 입력한 아이디를키로 사용해 시스템에 저장된 값을 불러온다
        string pass = PlayerPrefs.GetString(id.text);
        //만일 사용자가 입력한 패스워드와 시스템에서 불러온 값을 비교해서 동일하다면
        if (password.text == pass)
        {
            //다음 씬(1번씬)을 로드한다
            SceneManager.LoadScene(1);
        }
        //두 데이터의 값이 다르면, 사용자 정보 불일치메세지를 남긴다
        else
        {
            notify.text = "입력하신 아이디와 패스워드가 일치하지 않습니다.";
        }
    }
}
