using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

using RoomInfo = ExitGames.Client.Photon.LoadBalancing.RoomInfo;
using RoomOptions = ExitGames.Client.Photon.LoadBalancing.RoomOptions;
using ClientState = ExitGames.Client.Photon.LoadBalancing.ClientState;
using System.Text.RegularExpressions;

public class MainMenu : MonoBehaviour {

  [Header("Buttons")]
  public Button spButton;
  public Button mpButton;
  public Button ctButton;

  [Header("Single player")]
  public int spSceneIndex = -1;
  public GameObject singlenetPrefab;

  [Header("Multi player")]
  public GameObject netPrefab;
  public GameObject netCanvas;

  [Space]
  public GameObject loadingBarCanvas;
  public Image loadingBarImage;

  [Space]
  public GameObject controlsPopup;
  public Button controlsCloseButton;

  [Space]
  public GameObject netStateCanvas;
  public TextMeshProUGUI netStateText;

  [Space]
  public Button startButton;
  public Button disconnectButton;

  [Space]
  public GameObject CJCanvas;
  public TextMeshProUGUI roomTextField;
  public Button createButton;
  public Button joinButton;

  [Space]
  public GameObject waitingCanvas;
  public TextMeshProUGUI waitingPartnerText;

  private void Start() {
    var t  = ClientEntity.ForceInitlazation;

    spButton.onClick.RemoveAllListeners();
    spButton.onClick.AddListener(StartGame);

    mpButton.onClick.RemoveAllListeners();
    mpButton.onClick.AddListener(ConnectToJoinedState);

    ctButton.onClick.RemoveAllListeners();
    ctButton.onClick.AddListener(() => controlsPopup.SetActive(true));

    controlsCloseButton.onClick.RemoveAllListeners();
    controlsCloseButton.onClick.AddListener(() => controlsPopup.SetActive(false));
  }

  private void SetMainVis(bool showing) {
    spButton.gameObject.SetActive(showing);
    mpButton.gameObject.SetActive(showing);
    ctButton.gameObject.SetActive(showing);
  }

  private void StartGame() {
    if (spSceneIndex == -1) {
      Debug.LogError("No single player scene!");
      return;
    }

    RemoveAllListeners();

    if (NetworkManager.net == null)
      Instantiate(singlenetPrefab);

    StartCoroutine(DisplayLoadBar());
    LevelLoader.instance.RaiseStartGame(spSceneIndex);
  }

  private IEnumerator DisplayLoadBar(){
    loadingBarCanvas.SetActive(true);

    loadingBarImage.fillAmount = 0.0f;
    while (true){
      yield return null;
      loadingBarImage.fillAmount = LevelLoader.instance.progress;
    }
  }

  private void ConnectToJoinedState() {
    StartCoroutine(ConnectSequence());
  }

  private IEnumerator ConnectSequence() {
    Instantiate(netPrefab);
    netCanvas.SetActive(true);
    netStateCanvas.SetActive(true);

    SetMainVis(false);

    while (!NetworkManager.onNameServer || !NetworkManager.isReady) {
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }
    Debug.Log("Connected to name server");

    var regionRequest = NetworkManager.net.OpGetRegions();
    if (!regionRequest){
      Debug.Log("Failed request regions");
      yield break;
    }

    while (NetworkManager.net.AvailableRegions == null) {
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }
    Debug.Log("Received region list");

    NetworkManager.net.ConnectToRegionMaster("usw");
    while (!NetworkManager.onMasterLobby) {
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }

    netStateCanvas.SetActive(false);

    CJCanvas.SetActive(true);
    createButton.onClick.RemoveAllListeners();
    createButton.onClick.AddListener(CreateLobby);

    joinButton.onClick.RemoveAllListeners();
    joinButton.onClick.AddListener(JoinLobby);

    startButton.onClick.RemoveAllListeners();
    startButton.onClick.AddListener(StartGame);

    disconnectButton.onClick.RemoveAllListeners();
    disconnectButton.onClick.AddListener(Disconnect);
  }

  private void CreateLobby() {
    Debug.Log("Creating room");

    var ro = new RoomOptions();
    ro.EmptyRoomTtl = 1000;
    ro.CleanupCacheOnLeave = true;
    ro.PlayerTtl = 15000;
    ro.PublishUserId = false;
    ro.MaxPlayers = 2; // TODO: Expose this better

    string roomCode = string.Empty;
    var roomList = NetworkManager.net.RoomInfoList.Keys.ToList();
    do {
      roomCode = string.Format("{0}{1}{2}{3}", RandomChar(), RandomChar(), RandomChar(), RandomChar());
    } while (roomList.Contains(roomCode));

    var success = NetworkManager.net.OpCreateRoomWithProperties(roomCode, ro, ExitGames.Client.Photon.LoadBalancing.TypedLobby.Default, "lobby");
    if (success) {
      Debug.Log("Room created");
      StartCoroutine(WaitingOnRoom());
    } else {
      Debug.Log("Couldn't create room");
    }
  }

  private void JoinLobby() {
    var roomCode = roomTextField.text.ToUpperInvariant().Replace("\u200b", "");

    if (NetworkManager.net.OpJoinRoom(roomCode)) {
      Debug.Log("Room joined");
      StartCoroutine(WaitingOnRoom());
    } else {
      Debug.Log("Couldn't join room");
    }

  }

  private IEnumerator WaitingOnRoom() {
    CJCanvas.SetActive(false);
    netStateCanvas.SetActive(true);

    while (NetworkManager.net.State == ClientState.Joining) {
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }

    if (NetworkManager.net.State == ClientState.JoinedLobby) {
      CJCanvas.SetActive(true);
      netStateText.text = "Couldn't find room";

      var timeout = 1.0f;
      while (timeout > 0) {
        yield return null;
        timeout -= Time.deltaTime;
      }
      netStateCanvas.gameObject.SetActive(false);
      yield break;
    }

    while (NetworkManager.net.State != ClientState.Joined) {
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }

    netStateCanvas.SetActive(false);

    StartCoroutine(WaitingOnPlayer());

  }

  private IEnumerator WaitingOnPlayer() {
    waitingCanvas.SetActive(true);

    var waitingAnimation = 0.6f;
    var waitingEnd = string.Empty;
    while (NetworkManager.inLobby) {
      if (NetworkManager.net.CurrentRoom.PlayerCount != 2) {
        waitingAnimation -= Time.deltaTime;
        if (waitingAnimation < 0) {
          waitingEnd = new string('.', (waitingEnd.Length + 1) % 4);
          waitingAnimation += 0.6f;
        }
        waitingPartnerText.text = string.Format("waiting{0}", waitingEnd);
        startButton.gameObject.SetActive(false);
      } else {
        waitingPartnerText.text = "MARIBEL";
        startButton.gameObject.SetActive(true);
      }
      yield return null;
    }

  }

  private char RandomChar() {
    return (char)(UnityEngine.Random.Range(0, 26) + 'A');
  }

  private void Disconnect(){
    StartCoroutine(DisconnectSequence());
  }

  private IEnumerator DisconnectSequence(){
    NetworkManager.net.Service();
    NetworkManager.net.Disconnect();

    netStateCanvas.gameObject.SetActive(true);
    CJCanvas.SetActive(false);
    waitingCanvas.SetActive(false);
    startButton.gameObject.SetActive(false);

    while (NetworkManager.net.State != ClientState.Disconnected){
      netStateText.text = NetworkManager.net.State.ToString();
      yield return null;
    }

    SetMainVis(true);

    Destroy(NetworkManager.instance.gameObject);

    netStateCanvas.gameObject.SetActive(false);
    netCanvas.SetActive(false);
  }

  private void RemoveAllListeners(){
    spButton.onClick.RemoveAllListeners();
    mpButton.onClick.RemoveAllListeners();
  }

}
