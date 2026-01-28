using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    public GameObject Slime;
    public Transform Player;
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Slime.GetComponent<Slime>().HP <= 0) Destroy(gameObject);
        if (Slime != null && Player != null)
        {
            Vector3 slimePos = Slime.transform.position;
            Vector3 followPos = new Vector3(slimePos.x, slimePos.y + 0.9f, slimePos.z);

            transform.position = followPos;
            transform.LookAt(Player);
        }

        gameObject.GetComponent<Slider>().value = Slime.GetComponent<Slime>().HP;
    }
}
