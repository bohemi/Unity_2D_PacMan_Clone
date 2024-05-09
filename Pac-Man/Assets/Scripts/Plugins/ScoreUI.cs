using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    TMP_Text textScore;

    private void Start()
    {
        textScore = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textScore.text = Consumables.updatePoints.ToString();
    }
}
