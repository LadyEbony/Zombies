using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectDebug : MonoBehaviour {

  private TextMeshProUGUI textMesh;

  private void Awake() {
    textMesh = GetComponentInChildren<TextMeshProUGUI>();
  }

  // Update is called once per frame
  void Update() {
    if (NetworkManager.inRoom){
      string t = string.Empty;
      var room = NetworkManager.net.CurrentRoom;
      foreach (var p in room.Players){
        t = string.Format("{0}{1}: {2}\n", t, p.Key, p.Value.ID);
      }
      textMesh.text = t;
    } else {
      textMesh.text = "Offline";
    }
  }
}
