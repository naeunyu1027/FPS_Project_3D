using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHPbar : MonoBehaviour
{
    public GameObject HPbarPrefab;
    public GameObject SlimeHPbarSpawn;
    public void Spawn()
    {
        SlimeHPbarSpawn = GameObject.FindWithTag("SlimeHPbarSpawn");
        GameObject HPbar = Instantiate(HPbarPrefab, transform.position, Quaternion.identity);
        HPbar.transform.SetParent(SlimeHPbarSpawn.transform, worldPositionStays: true);
        HPbar.GetComponent<HPbar>().Slime = gameObject;
    }

    void Update()
    {
        
    }
}
