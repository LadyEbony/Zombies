using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTimer : KTimer {
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public enum ListSelectionOrder { CONSECUTIVE, RANDOM }
    public ListSelectionOrder audioClipOrder;
    public bool preventConsecutiveRepeat;   // Prevents the same sound from playing two times in a row (when randomizeOrder is enabled)
    private int currentClipIndex;

    protected void playSound(AudioClip clip) {
        if(clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    protected AudioClip chooseClip() {
        if(audioClips.Length == 0) {
            return null;
        }
        switch(audioClipOrder) {
            case ListSelectionOrder.CONSECUTIVE:
                currentClipIndex = (currentClipIndex + 1) % audioClips.Length;
                break;
            case ListSelectionOrder.RANDOM:
                if(preventConsecutiveRepeat && audioClips.Length > 1) {
                    int i;
                    while((i = Random.Range(0, audioClips.Length)) == currentClipIndex);
                    currentClipIndex = i;
                }
                else {
                    currentClipIndex = Random.Range(0, audioClips.Length);
                }
                break;
            default:
                Debug.LogWarning("Unhandled ListSelectionOrder \"" + audioClipOrder.ToString() + "\" in switch statement");
                break;
        }
        return audioClips[currentClipIndex];
    }

    protected override void TimedAction() {
        playSound(chooseClip());
    }
}