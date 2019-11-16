using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon.LoadBalancing;

public class QuickJoin : MonoBehaviour {
  // Update is called once per frame
  public bool allowQuickJoin = true;

  private TextMeshProUGUI textMesh;

  private void Awake() {
    textMesh = GetComponentInChildren<TextMeshProUGUI>();
  }

  void Update () {
    if (allowQuickJoin && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.J)) {
      if (NetworkManager.inRoom) return;

      var activeScene = SceneManager.GetActiveScene();

      var ro = new RoomOptions();
      ro.IsVisible = false;
      ro.IsOpen = true;
      ro.MaxPlayers = NetworkManager.instance.expectedMaxPlayers;

      var success = NetworkManager.net.OpJoinOrCreateRoomWithProperties(activeScene.name, ro, null, "game");
      Debug.Log(success);
    }

    //if (NetworkManager.instance)
    if (NetworkManager.net != null) {
      currentState = NetworkManager.net.State;
    }

    if (textMesh){
      var room = NetworkManager.net.CurrentRoom;
      var player = NetworkManager.net.LocalPlayer;
      var playerCount = room != null ? string.Format("{0}/{1}", room.PlayerCount, room.MaxPlayers) : "0/0";
      textMesh.text = string.Format("{0}. {1}. {2}", currentState.ToString(), playerCount, player.ID);
    }
  }

  IEnumerator Start() {
    // Wait until the name server is reached
    while (!NetworkManager.onNameServer || !NetworkManager.isReady) yield return null;
    //while (!NetworkManager)
    getRegions();
  }

  [ContextMenu("Get Regions")]
  void getRegions() {
    var regionRequest = NetworkManager.net.OpGetRegions();
    if (regionRequest) {
      StartCoroutine(handleRegionListWhenAvailable());
      Debug.Log("Region request sent");
    } else {
      Debug.Log("Failed request regions");
    }
  }


  IEnumerator handleRegionListWhenAvailable() {
    while (NetworkManager.net.AvailableRegions == null) yield return null;
    Debug.Log("Regions list recieved");

    //var regions = NetworkManager.net.AvailableRegions;

    NetworkManager.net.ConnectToRegionMaster(NetworkManager.net.AvailableRegions[0]);
  }

  // Debug out the current state for visibility
  [Header("Current state")]
	public ExitGames.Client.Photon.LoadBalancing.ClientState currentState;
}
