using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    //에너미 상태 변수
    private EnemyState m_State;

    //플레이어 발견 범위
    public float findDistance;

    //플레이어의 트렌스폼
    private Transform player;

    //공격 가능 범위
    public float attackDistance = 2f;

    //이동 속도
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
    public int hp = 15;
    private int maxHP = 15;
    public Slider hpSlider;

    private Animator anim;
    //네비게이션 에이전트
    private NavMeshAgent smith;

    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //현재 상태를 체크해 해당 상태별로 정해진 기능 수행
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                Damaged();
                break;
            case EnemyState.Die:
                Die();
                break;
        }
        hpSlider.value = (float)hp / (float)maxHP;
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            Debug.Log("상태전환 : Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }

    private void Move()
    {
        //이동 범위를 벗어난다면
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            Debug.Log("상태 변환 : move -> Return");
        }
        //만일 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            /*Vector3 dir = (player.position - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            
            //플레이어를 향해 방향 전환
            transform.forward = dir;*/
            
            //네이게이션 에이전트의 이동을 멈추고 경로 초기호ㅓ
            smith.isStopped = true;
            smith.ResetPath();
            
            //네비게이션으로 접근하는 최소 거리를 공격가능 거리로 설정한다.
            smith.stoppingDistance = attackDistance;
            //네비게이션의 목적지는 플레이어 위치
            smith.destination = player.position;
        }
        else
        {
            m_State = EnemyState.Attack;
            Debug.Log("상태 변환 : Move -> Attack");
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
            m_State = EnemyState.Move;
            Debug.Log("상태 전환 : Attack -> Move");
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }

    public void AttackAction()
    {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }
    
    private void Return()
    {
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            /*Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);

            transform.forward = dir;*/
            
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
            hp = maxHP;
            m_State = EnemyState.Idle;
            Debug.Log("상태 변환 : Return -> Idel");
            anim.SetTrigger("MoveToIdle");
        }
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
    private void Damaged()
    {
        StartCoroutine(DamageProces());
    }


    IEnumerator DamageProces()
    {
        yield return new WaitForSeconds(1f);
        m_State = EnemyState.Move;
        Debug.Log("상태 변환 : Damaged -> Move");
    }

    // 데미지 실행 메서드
    public void HitEnemy(int hitPower)
    {
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        hp -= hitPower;
        
        // 네비게이션 에이전트의 이동을 멈추고 경로 초기화
        smith.isStopped = true;
        smith.ResetPath();

        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            Debug.Log("상태 전환 : AnyState -> Damaged");

            anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            m_State = EnemyState.Die;
            Debug.Log("상태 전환 : AnyState -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
