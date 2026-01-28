using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 200f; //회전 속도

    private float mx = 0;
    private float my = 0;
    void Start()
    {
        
    }

    void Update()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게 한다
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        
        //마우스 입력 -> 물체 회전
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");
        
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;

        my = Mathf.Clamp(my, -90f, 90f);

        /*Vector3 dir = new Vector3(-my, mx, 0);*/
        /*transform.eulerAngles += dir * rotSpeed * Time.deltaTime; // r = r0 + vt*/
        transform.eulerAngles = new Vector3(-my, mx, 0);
        //제한
        /*Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);
        transform.eulerAngles = rot;*/
    }
}
