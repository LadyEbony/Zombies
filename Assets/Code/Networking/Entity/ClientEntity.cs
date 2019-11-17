using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using EntityNetwork;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

using Player = ExitGames.Client.Photon.LoadBalancing.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class ClientEntity  {

  public static readonly string characterSelected = "cs";
  public static readonly string readyStatus = "rs";

  public static readonly string sceneStatus = "ss";

  public static bool ForceInitlazation;

  public static Player localPlayer {
     get {
        return NetworkManager.net.LocalPlayer;
     }
  }

  public static void CreatePlayerHashtable(){
    var h = new Hashtable();

    h.Add(characterSelected, -1);
    h.Add(readyStatus, false);
    h.Add(sceneStatus, false);

    localPlayer.SetCustomProperties(h);
  }

  public static void SetRoomState(string state){
    var k = new Hashtable();
    k.Add(PhotonConstants.propScene, state);
    NetworkManager.net.OpSetPropertiesOfRoom(k);
  }

  public static void SetCharacter(int i){
    var k = new Hashtable();
    k.Add(characterSelected, i);
    localPlayer.SetCustomProperties(k);
  }

  public static int GetCharacter(Player p){
    return (int)p.CustomProperties[characterSelected];
  }

  public static void SetReadyStatus(bool status){
    var k = new Hashtable();
    k.Add(readyStatus, status);
    localPlayer.SetCustomProperties(k);
  }

  public static bool GetReadyStatus(Player p){
    return (bool)p.CustomProperties[readyStatus];
  }

  public static bool GetRoomReadyStatus(){
    var players = NetworkManager.net.CurrentRoom.Players.Values;
    foreach(var p in players){
      if (!GetReadyStatus(p)) return false;
    }
    return true;
  }

  public static void SetSceneStatus(bool status){
    var k = new Hashtable();
    k.Add(sceneStatus, status);
    localPlayer.SetCustomProperties(k);
  }

  public static bool GetSceneStatus(Player p){
    return (bool)p.CustomProperties[sceneStatus];
  }  

}
