using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 7f;
    private CharacterController cc;
    private float gravity = -20f;
    private float yVelocity = 0;
    public float jumpPower = 10f;
    public bool isJumping = false;
    public int hp = 20;
    //플레이어 최대 체력
    private int maxHP = 20;
    //hp슬라이더 변수
    public Slider hpSlider;
    //HIT효과
    public GameObject hitEffect;
    private Animator anim;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게 한다
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        
        
         //사용자 입력 받기
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        //이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        
        // 이동 벌랜딩 애니메이션 호툴하고 백터의 크기 값음을넘겨준다
        anim.SetFloat("MoveMotion", dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir); //메인 카메라 기준 방향 변환

        if (isJumping && cc.collisionFlags == CollisionFlags.Below)
        {
            isJumping = false;
            yVelocity = 0;
        } 
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }
        
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;
        
        //이동
        /*transform.position += dir * moveSpeed * Time.deltaTime;*/
        cc.Move(dir * moveSpeed * Time.deltaTime);
        
        //현재 플레이어 hp를 hp슬라이더의 벨류에 반영
        hpSlider.value = (float)hp / (float)maxHP;
    }

    public void DamageAction(int damage)
    {
        hp -= damage;
        if (hp > 0) StartCoroutine(PlayerHitEffect());
    }

    IEnumerator PlayerHitEffect()
    {
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitEffect.SetActive(false);
    }
}
