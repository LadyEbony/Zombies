using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KTimer : MonoBehaviour {
    public float timeInterval;  
    private enum AudioTimerState { AT_WAIT, AT_PLAY }
    private AudioTimerState state;
    private float timer;

    [HideInInspector]
    public bool Playing = false;

    protected abstract void TimedAction();

    protected virtual void Start() {
        state = AudioTimerState.AT_WAIT;
        timer = 0;
    }
    
    protected virtual void Update() {
        switch(state) {
            case AudioTimerState.AT_WAIT:
                if(Playing) {
                    timer = 0;
                    state = AudioTimerState.AT_PLAY;
                    TimedAction();
                }
                break;
            case AudioTimerState.AT_PLAY:
                if(Playing) {
                    timer += Time.deltaTime;
                    if(timer >= timeInterval) {
                        timer = 0;
                        TimedAction();
                    }
                }
                else {
                    timer = 0;
                    state = AudioTimerState.AT_WAIT;
                }
                break;
            default:
                Debug.LogWarning("Unhandled AudioTimerState \"" + state.ToString() + "\" in switch statement");
                break;
        }
    }
}