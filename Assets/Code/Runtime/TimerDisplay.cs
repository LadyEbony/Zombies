using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI), typeof(Timer))]
public class TimerDisplay : MonoBehaviour {

  public static TimerDisplay Instance;

  private Timer timer;
  private TextMeshProUGUI textMesh;

  private void Awake() {
    Instance = this;

    timer = GetComponent<Timer>();
    textMesh = GetComponent<TextMeshProUGUI>();
  }

  private IEnumerator Start() {
    while (!PlayerSpawner.gameReady) yield return null;
    timer.running = true;
  }

  private void Update() {
    if (!PlayerSpawner.gameReady) return;

    if (PlayerController.Local == null || !PlayerController.Local.gameObject.activeSelf) {
      timer.running = false;
      textMesh.color = Color.red;
      Destroy(this);
    }

    textMesh.text = timer.ToString();
  }
}
