using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class RoomCodeDisplay : MonoBehaviour {

  public GameObject Display;
  public TextMeshProUGUI Text;

  private void Awake() {
    //DontDestroyOnLoad(gameObject);
  }

  private bool lastGameBool;
  void Update () {
    // Detected change
	 if (lastGameBool != NetworkManager.inLobby){
      if (NetworkManager.inLobby) {
        Display.SetActive(true);
        Text.text = NetworkManager.net.CurrentRoom.Name;
      } else {
        Display.SetActive(false);
      }
    }
    lastGameBool = NetworkManager.inLobby;

/*    if (NetworkManager.net != null)
    if ((NetworkManager.net.CurrentRoom != null) && (NetworkManager.net.CurrentRoom.PlayerCount > 1))
      Destroy(this);*/
	}
}
