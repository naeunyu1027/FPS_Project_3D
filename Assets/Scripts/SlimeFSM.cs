using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SlimeFSM : MonoBehaviour
{
    [SerializeField] Vector3 speed;
    private IObjectPool<SlimeFSM> slimePool;



    public void SetslimePool(IObjectPool<SlimeFSM> pool)
    {
        slimePool = pool;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime;
        if (HP <= 0)
        {
            DisableSilme();
        }

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

    // private void OnBecameInvisible()
    // {
    //     if (slimePool != null)
    //         slimePool.Release(this);
    // }
    public void DieSlime()
    {
        Invoke("DisableSilme", 5f);
    }

    private void DisableSilme()
    {
        slimePool.Release(this);
    }


    void Start()
    {
        s_State = SlimeState.Idle;
        anim = transform.GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
    }
    enum SlimeState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    private SlimeState s_State;
    public float FindDistance;
    private Transform player;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    private CharacterController cc;
    //누적 시간
    private float currentTime = 0f;
    //공격 딜레이 시간
    private float attackDelay = 2f;
    public int attackPower;
    private Vector3 originPos;
    //초기 회전 저장용 변수
    private Quaternion originRot;
    public float moveDistance = 20f;
    public int HP = 100;

    private int maxHP = 15;
    private Animator anim;

    private void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < FindDistance)
        {
            s_State = SlimeState.Move;
            Debug.Log("Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }
    private void Move()
    {
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            s_State = SlimeState.Return;
            Debug.Log("move -> Return");
        }
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);

            //플레이어를 향해 방향 전환
            transform.forward = dir;
        }
        else
        {
            s_State = SlimeState.Attack;
            Debug.Log("Move -> Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }
    private void Attack()
    {
        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            //일정 시간마다 플레이어를 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                // player.GetComponent<PlayerMove>().DamageAction(attackPower);
                Debug.Log("공격!");
                currentTime = 0f;
                anim.SetTrigger("StartAttack");
            }
        }
        else // 그렇지 않다면 현재 상태를 이동으로 전환
        {
            s_State = SlimeState.Move;
            Debug.Log("상태 전환 : Attack -> Move");
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }
    private void Return()
    {
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);

            transform.forward = dir;

            // //네비게이션의 목적지를 초기 저장된 위치로 설정
            // smith.destination = originPos;
            // //네비게이션으로 접근하는 최소 거리 '0'
            // smith.stoppingDistance = 0;
        }
        else
        {
            //네비게이션 에이전트의 이동을 멈추고 경로 초기화
            // smith.isStopped = true;
            // smith.ResetPath();

            transform.position = originPos;
            HP = maxHP;
            s_State = SlimeState.Idle;
            Debug.Log("상태 변환 : Return -> Idel");
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
        Debug.Log("상태 변환 : Damaged -> Move");
    }

    private void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트를 비활성화
        cc.enabled = false;
        Debug.Log("DieProcess여까지옴");
        yield return new WaitForSeconds(2f);
        Debug.Log("소멸!");
        Destroy(gameObject);
    }
    
    public void HitEnemy(int hitPower)
    {
        if (s_State == SlimeState.Damaged || s_State == SlimeState.Die || s_State == SlimeState.Return)
        {
            return;
        }
        HP -= hitPower;
        
        // 네비게이션 에이전트의 이동을 멈추고 경로 초기화
        // smith.isStopped = true;
        // smith.ResetPath();

        if (HP > 0)
        {
            s_State = SlimeState.Damaged;
            Debug.Log("상태 전환 : AnyState -> Damaged");

            anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            s_State = SlimeState.Die;
            Debug.Log("상태 전환 : AnyState -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
