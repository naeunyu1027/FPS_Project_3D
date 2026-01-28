using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using UnityEngine.AI;
using TMPro;

public class Slime : MonoBehaviour
{
    [SerializeField] Vector3 speed;
    private IObjectPool<Slime> slimePool;

    public void SetslimePool(IObjectPool<Slime> pool)
    {
        slimePool = pool;
    }

    
    public int HP = 100;
    private int maxHP;
    public float FindDistance;
    public float attackDistance;
    public float moveDistance;
    public int attackPower;
    public GameObject poolManager;
    [SerializeField] private float moveSpeed = 4.9017f; 
    [SerializeField] private float attackDelay;
    private Transform player;
    private Animator anim;
    private SlimeState s_State;
    private Vector3 originPos;
    private Quaternion originRot;
    private CharacterController cc;
    private float currentTime = 0f;
    private GameObject slimeHPbar;
    private GameObject attackPlayer;
    private NavMeshAgent smith;
    private GameObject Score;

    enum SlimeState
    {
        Idle, Move, Attack, Return, Damaged, Die
    }
    public void Init()
    {
        player = GameObject.FindWithTag("Player").transform;
        poolManager = GameObject.FindWithTag("PoolManager");
        attackPlayer = GameObject.FindWithTag("attackPlayer");
        Score = GameObject.FindWithTag("Score");
        anim = GetComponentInChildren<Animator>();
        originPos = transform.position;
        cc = GetComponent<CharacterController>();
        gameObject.GetComponent<SlimeHPbar>().Spawn();
        s_State = SlimeState.Idle;
        isDieCalledOnce = false; // 풀에 반환되면 다시 초기화 
        smith = GetComponent<NavMeshAgent>();

    }

    void Reset()
    {
 
        if (smith != null)
        {
            // smith.ResetPath();
            smith.isStopped = false;
        }
        if (anim != null)
        {
            anim.Rebind(); // 초기 바인딩으로 리셋
            // anim.Update(0f);
        }
    //     foreach (Transform child in transform)
    // {
    //     child.localPosition = Vector3.zero;
    //     child.localRotation = Quaternion.identity;
    //     child.localScale = Vector3.one;
    // }
    }



    private void DisableSilme()
    {
        slimePool.Release(this);
        poolManager.GetComponent<PoolManager>().Invoke("GetPoolSlime", 4f);

    }

    void Update()
    {
        
        transform.position += speed * Time.deltaTime;
        switch (s_State)
        {
            case SlimeState.Idle:
                Idle();
                break;
            case SlimeState.Move:
                Move();
                break;
            case SlimeState.Attack:
                Attack();
                break;
            case SlimeState.Return:
                Return();
                break;
            case SlimeState.Damaged:
                Damaged();
                break;
            case SlimeState.Die:
                Die();
                break;
        }
    }
    private void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < FindDistance)
        {
            s_State = SlimeState.Move;
            // UnityEngine.Debug.Log("Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }

    private void Move()
    {
        // if (Vector3.Distance(transform.position, originPos) > moveDistance)
        // {
        //     // s_State = SlimeState.Return;
        //     // UnityEngine.Debug.Log("move -> Return");
        // }
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // Vector3 dir = (player.position - transform.position).normalized;
            // cc.Move(dir * moveSpeed * Time.deltaTime);
            // transform.forward = dir;

            smith.isStopped = true;
            smith.ResetPath(); // 이거 안하면 안움직임임

            smith.stoppingDistance = attackDistance;
            smith.destination = player.position;
        }
        else
        {
            s_State = SlimeState.Attack;
            // UnityEngine.Debug.Log("Move -> Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            //일정 시간마다 플레이어를 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                // UnityEngine.Debug.Log("공격!");
                currentTime = 0f;
                anim.SetTrigger("StartAttack");
                AttackAction();
            }
        }
        else // 그렇지 않다면 현재 상태를 이동으로 전환
        {
            s_State = SlimeState.Move;
            // UnityEngine.Debug.Log("상태 전환 : Attack -> Move");
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }
    public void AttackAction()
    {
        attackPlayer.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    private void Return()
    {   
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            // Vector3 dir = (originPos - transform.position).normalized;
            // cc.Move(dir * moveSpeed * Time.deltaTime);

            // transform.forward = dir;

            //네비게이션의 목적지를 초기 저장된 위치로 설정
            smith.destination = originPos;
            //네비게이션으로 접근하는 최소 거리 '0'
            smith.stoppingDistance = 0;
        }
        else
        {
            //네비게이션 에이전트의 이동을 멈추고 경로 초기화
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            HP = maxHP;
            s_State = SlimeState.Idle;
            // UnityEngine.Debug.Log("상태 변환 : Return -> Idel");
            anim.SetTrigger("MoveToIdle");
        }
    }
    private void Damaged()
    {
        StartCoroutine(DamageProces());
    }


    IEnumerator DamageProces()
    {
        yield return new WaitForSeconds(1f);
        s_State = SlimeState.Move;
        // UnityEngine.Debug.Log("상태 변환 : Damaged -> Move");
    }

    public bool isDieCalledOnce = false;
    private void Die()
    {
        if (isDieCalledOnce) return; // 이미 실행된 경우 무시
        isDieCalledOnce = true;      // 첫 실행 이후로 막음

        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // UnityEngine.Debug.Log("DieProcess");
        // 캐릭터 컨트롤러 컴포넌트를 비활성화
        // cc.enabled = false;
        // StartCoroutine(poolManager.GetComponent<PoolManager>().GetPoolSlime());
        yield return new WaitForSeconds(2f);
        DisableSilme();
        Score.GetComponent<Score>().Point += 1;
        Score.GetComponent<Score>().ScoreText();
        yield return new WaitForSeconds(4f);
    }

    public void HitEnemy(int hitPower)
    {
        if (s_State == SlimeState.Damaged || s_State == SlimeState.Die || s_State == SlimeState.Return)
        {
            return;
        }
        HP -= hitPower;

        // 네비게이션 에이전트의 이동을 멈추고 경로 초기화
        smith.isStopped = true;
        smith.ResetPath();

        if (HP > 0)
        {
            s_State = SlimeState.Damaged;
            // UnityEngine.Debug.Log("상태 전환 : AnyState -> Damaged");

            anim.SetTrigger("Damaged");
            Damaged();
        }
        else if (HP <= 0)
        {
            s_State = SlimeState.Die;
            // UnityEngine.Debug.Log("상태 전환 : AnyState -> Die");
            anim.SetTrigger("Die");
        }
    }
    
}
 