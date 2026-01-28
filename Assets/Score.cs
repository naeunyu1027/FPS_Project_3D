using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public int Point = 0;
    private TextMeshProUGUI tmp;
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        ScoreText();
    }

    void Update()
    {

    }

    public void ScoreText()
    {
        tmp.text = "Score : " + Point;
    }
}
