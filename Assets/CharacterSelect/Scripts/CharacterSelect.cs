using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterSelect : MonoBehaviour {

  public GameObject playerPrefab;
  public Transform playerTranform;
  public Dictionary<int, CharacterSelectPortrait> playerDictionary;

  public static CharacterSelect Instance { get; private set; }

  private void Awake() {
    Instance = this;
  }

  private IEnumerator Start() {
    Transform t;

    t = transform.Find("Players");
    playerTranform = t;
    playerDictionary = new Dictionary<int, CharacterSelectPortrait>();
    playerPrefab = t.GetChild(0).gameObject;
    playerPrefab.SetActive(false);

    ClientEntity.CreatePlayerHashtable();
    bool request;

    while (!NetworkManager.onNameServer || !NetworkManager.isReady) yield return null;

    request = NetworkManager.net.OpGetRegions();
    if (request) {
      Debug.Log("Region request sent");
    } else {
      Debug.Log("Failed request regions");
      yield break;
    }

    while (NetworkManager.net.AvailableRegions == null) yield return null;
    Debug.Log("Regions list recieved");

    request = NetworkManager.net.ConnectToRegionMaster(NetworkManager.net.AvailableRegions[0]);
    if (request) {
      Debug.Log("Connected to region master.");
    } else {
      Debug.Log("Couldn't connect to region master.");
      yield break;
    }

    while (!NetworkManager.onMasterLobby) yield return null;

    var ro = new RoomOptions();
    ro.EmptyRoomTtl = 1000;
    ro.CleanupCacheOnLeave = true;
    ro.PlayerTtl = 500;
    ro.PublishUserId = false;
    ro.MaxPlayers = 20; // TODO: Expose this better

    request = NetworkManager.net.OpJoinOrCreateRoom("gamespawn", ro, ExitGames.Client.Photon.LoadBalancing.TypedLobby.Default);
    if (request) {
      Debug.Log("Room created");
    } else {
      Debug.Log("Couldn't create/join room");
      yield break;
    }

    while (!NetworkManager.inRoom) yield return null;

    // Online
    // Create portraits based on who's connected
    if (NetworkManager.inRoom){
      var players = NetworkManager.getSortedPlayers;
      foreach (var p in players) {
        AddPlayer(p.ID);
      }
    } else {
      Debug.LogError("not online wtf");
    }

    // Create buttons based on the existing prefab
    t = transform.Find("Buttons");
    var buttonPrefab = t.GetComponentInChildren<ButtonCS>().gameObject;
    var buttonTransform = t.Find("Characters");
    var characters = CharacterManager.Instance.GetCharacters;
    var charactersLength = characters.Length;
  
    for(var i = 0; i < charactersLength; i ++){
      var gobject = Instantiate(buttonPrefab, buttonTransform);
      var comp = gobject.GetComponent<ButtonCS>();
      comp.SetCharacter(characters[i], i);
    }

    buttonPrefab.SetActive(false);
  }

  public void LoadGame(){
    SceneManager.LoadScene("BoardStagingOnline");
  }

  private void OnEnable() {
    NetworkManager.onJoin += OnPlayerJoin;
    NetworkManager.onLeave += OnPlayerLeave;
  }

  private void OnDisable() {
    NetworkManager.onJoin -= OnPlayerJoin;
    NetworkManager.onLeave -= OnPlayerLeave;
  }

  private void OnPlayerJoin(EventData data) {
    var id = (int)data.Parameters[ParameterCode.ActorNr];
    AddPlayer(id);
  }

  private void OnPlayerLeave(EventData data) {
    var id = (int)data.Parameters[ParameterCode.ActorNr];

    if (playerDictionary.ContainsKey(id)) {
      Destroy(playerDictionary[id].gameObject);
      playerDictionary.Remove(id);

      ReadyCheck.Stop = true;
    }
  }

  private void AddPlayer(int id){
    if (!playerDictionary.ContainsKey(id)) {
      var gobject = Instantiate(playerPrefab, playerTranform);
      gobject.SetActive(true);

      var comp = gobject.GetComponent<CharacterSelectPortrait>();
      var player = NetworkManager.net.CurrentRoom.GetPlayer(id);
      comp.SetPlayer(player);

      playerDictionary.Add(id, comp);

      ReadyCheck.Stop = true;
    }
  }

}
