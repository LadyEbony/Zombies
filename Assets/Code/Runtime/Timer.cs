using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

  public float currTime { get; private set; }
  public bool running = true;

  private void Awake() {
    currTime = 0f;
  }

  private void Update() {
    if (running) currTime += Time.deltaTime;
  }

  public override string ToString() {
    const float MINUTE = 60f;
    const float HOUR = 3600f;
    float t = currTime;
    int h, m, s;

    if (t >= HOUR){
      h = Mathf.FloorToInt(t / HOUR);
      t -= h * HOUR;
    } else {
      h = 0;
    }

    if (t >= MINUTE){
      m = Mathf.FloorToInt(t / MINUTE);
      t -= h * MINUTE;
    } else {
      m = 0;
    }

    s = Mathf.FloorToInt(t);

    return string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
  }
}