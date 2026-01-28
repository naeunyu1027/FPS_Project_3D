using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;

    //수류탄 데미지
    public int attackPower = 10;
    //폭발 효과 반경
    public float explosionRadius = 5f;
    void Start()
    {
                
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collion)
    {
        //폭발 효과 반경 내에서 레이어가 'Enemy'인 모든 게임 오브젝트들의 Collier 컴포넌트를 배열에 저장한다
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius, 1 << 10);

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<Slime>().HitEnemy(attackPower);
        }
        
        GameObject eff = Instantiate(bombEffect);
        eff.transform.position = transform.position;
        Destroy(gameObject);
    }
}
