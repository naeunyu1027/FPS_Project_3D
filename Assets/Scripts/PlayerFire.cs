using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public GameObject firePosition;
    public GameObject bombFactory;
    public float throwPower = 15f;
    public GameObject bulletEffect;
    private ParticleSystem ps;
    public int weaponPower = 10;
    private Animator anim;

    //무기 모드 변수
    enum WeaponMode
    {
        Normal,
        Sniper
    }

    private WeaponMode wMode;

    //카메라 확대 확인용 변수
    private bool ZoomMode = false;

    //무기 모드 텍스트
    public Text wModeText;

    //총 발사 효과 오브젝트 배열
    public GameObject[] eff_Flash;
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        //무기 기본모드를 노멀 모드로 설정한다.
        wMode = WeaponMode.Normal;
        wModeText.text = "Normal Mode";
    }

    // Update is called once per frame
    void Update()
    {
        //게임 상태가 '게임 중' 상태일때만 조작할 수 있게 한다
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    //수류탄 오브젲ㄱ트를 생성한 후 
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    Rigidbody rb = bomb.GetComponent<Rigidbody>();
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);

                    float x, y, z;
                    x = Random.Range(0f, 1f);
                    y = Random.Range(0f, 1f);
                    z = Random.Range(0f, 1f);
                    Vector3 direction = new Vector3(x, y, x);
                    rb.AddTorque(direction * 0.2f, ForceMode.Impulse);

                    break;
                case WeaponMode.Sniper:
                    // 만일 줌 모드 상태가 아니라면 카메라를 확대하고 줌 모드 상태로 변경
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    else
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = false;
                    }
                    break;
            }
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }

        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootEffectOn(0.3f));

            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Slime slime = hitInfo.transform.GetComponent<Slime>();
                    slime.HitEnemy(weaponPower);
                }
                bulletEffect.transform.position = hitInfo.point;
                bulletEffect.transform.forward = hitInfo.normal;
                ps.Play();
            }

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            Camera.main.fieldOfView = 60f;
            wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;
            wModeText.text = "Sniper Mode";
        }
    }

    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length - 1);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }
}
