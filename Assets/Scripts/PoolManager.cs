using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private int maxSize = 20;
    [SerializeField] private int minSize = 15;
    [SerializeField] Slime SlimePrefab;
    private IObjectPool<Slime> _slimePool;
    public GameObject rangeObject;
    BoxCollider rangeCollider;
    // public GameObject _slime;

    void Awake()
    {
        _slimePool = new ObjectPool<Slime>(
            CreateSlime,
            ActivatePoolSlime,
            DisablePoolSlime,
            DestroyPoolSlime,
            false,
            minSize,
            maxSize: maxSize);
    }


    // void Update()
    // {
    //     for (int i = 0; i < maxSize; i++)
    //     {
    //         _slimePool.Get();
    //     }

    // }
    void Start()
    {
        rangeCollider = rangeObject.GetComponent<BoxCollider>();
        for (int i = 0; i < minSize; i++)
        {
            var slime = _slimePool.Get();
            slime.GetComponent<Slime>().Init();
        }
    }

    private Slime CreateSlime()
    {

        Slime slime = Instantiate(SlimePrefab, Return_RandomPosition(), Quaternion.identity).GetComponent<Slime>();
        slime.SetslimePool(_slimePool);
        return slime;
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, 0f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    private void ActivatePoolSlime(Slime slime)
    {
        // Debug.Log("ActivatePoolSlime");
        slime.HP = 100;
        slime.gameObject.SetActive(true);
    }

    private void DisablePoolSlime(Slime slime)
    {

        // Debug.Log("DisablePoolSlime");
        if (slime.HP <= 0)
        {
            slime.gameObject.SetActive(false);
        }
    }

    private void DestroyPoolSlime(Slime slime)
    {
        // Debug.Log("DestroyPoolSlime");
        Destroy(slime.gameObject);
    }

    public void GetPoolSlime()
    {
        var slime = _slimePool.Get();
        slime.GetComponent<Slime>().Init();
    }
}
