using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    public Timer timer;
    public float time;
    public TextMeshProUGUI timerText;

    protected void Start() {
        timer.startTimer();
    }

    protected void Update() {
        timerText.text = timer.timeFromStart();
    }
}
