using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private float startTime;
    private float currTime;
    public System.Action OnFinish = delegate {};
    private bool running;

    public void startTimer () {
        startTime = Time.time;
        running = true;
    }

    public void stopTimer () {
        running = false;
    }

    public float secondsFromStart () {
        if (running) {
            return Mathf.Clamp (currTime - startTime, 0, Mathf.Infinity);
        } else {
            return 0;
        }
    }

    public string timeFromStart () {
        const float MINUTE = 60f;
        const float HOUR = 3600f;
        string s = "";
        float t = secondsFromStart();
        float temp;

        // HOURS
        if(t >= HOUR) {
            temp = Mathf.Floor(t / HOUR);
            s += temp.ToString().PadLeft(2, '0') + ":";
            t -= temp * HOUR;
        }

        // MINUTES
        temp = Mathf.Floor(t / MINUTE);
        s += temp.ToString().PadLeft(2, '0') + ":";
        t -= temp * MINUTE;

        // SECONDS
        temp = Mathf.Floor(t);
        s += temp.ToString().PadLeft(2, '0');

        return s;
    }

    protected void Awake() {
        currTime = Time.time;
    }

    protected void Update () {
        if (running) {
            currTime = Time.time;
        }
    }
}