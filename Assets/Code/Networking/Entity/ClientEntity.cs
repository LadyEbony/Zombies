using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using EntityNetwork;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using Player = ExitGames.Client.Photon.LoadBalancing.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ClientEntityEntry<T> {
  private string id;

  public ClientEntityEntry(string id){
    this.id = id;
  }

  public T GetLocal(){
    return Get(NetworkManager.net.LocalPlayer);
  }

  public T Get(Player p){
    return (T)p.CustomProperties[id];
  }

  public void SetLocal(T value){
    Set(NetworkManager.net.LocalPlayer, value);
  }

  public void Set(Player p, T value){
    var h = new Hashtable();
    h.Add(id, value);
    p.SetCustomProperties(h);
  }

  public void Initialilze(Hashtable h, T value){
    h.Add(id, value);
  }

}

public static class ClientEntity  {
  public static ClientEntityEntry<int> characterHovered = new ClientEntityEntry<int>("ch");
  public static ClientEntityEntry<int> characterSelected = new ClientEntityEntry<int>("cs");

  public static ClientEntityEntry<bool> lobbyStatus = new ClientEntityEntry<bool>("ls");
  public static ClientEntityEntry<bool> gameStatus = new ClientEntityEntry<bool>("gs");

  public static ClientEntityEntry<int> randomValue = new ClientEntityEntry<int>("rv");

  public static bool ForceInitlazation;

  public static Player localPlayer {
     get {
        return NetworkManager.net.LocalPlayer;
     }
  }

  public static void CreatePlayerHashtable(){
    var h = new Hashtable();

    characterHovered.Initialilze(h, -1);
    characterSelected.Initialilze(h, -1);

    lobbyStatus.Initialilze(h, false);
    gameStatus.Initialilze(h, false);

    localPlayer.SetCustomProperties(h);
  }

  public static bool GetAllLobbyStatus(){
    var players = NetworkManager.net.CurrentRoom.Players.Values;
    foreach(var p in players){
      if (!lobbyStatus.Get(p)) return false;
    }
    return true;
  }

  public static bool GetAllGameStatus() {
    var players = NetworkManager.net.CurrentRoom.Players.Values;
    foreach (var p in players) {
      if (!gameStatus.Get(p)) return false;
    }
    return true;
  }

}
