using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon.LoadBalancing;
using System.Linq;

public class PlayerSpawner : MonoBehaviour {

  public static bool gameReady;

  public GameObject playerPrefab;
  private List<Transform> spawnPoints;

  // Start is called before the first frame update
  IEnumerator Start() {
    playerPrefab.SetActive(false);
    gameReady = false;

    spawnPoints = new List<Transform>();
    foreach (Transform child in transform)
      spawnPoints.Add(child);

    ClientEntity.CreateDummyPlayerHashtable();
    ClientEntity.gameStatus.SetLocal(true);

    while (!NetworkManager.inGamePlayersReady) yield return null;

    playerPrefab.SetActive(true);
    if (NetworkManager.inRoom) {
      var players = NetworkManager.getSortedPlayers;
      foreach (var p in players) {
        CreatePlayer(p);
      }
    } else {
      CreatePlayer(ClientEntity.localPlayer);
    }
    playerPrefab.SetActive(false);

    gameReady = true;

  }

  void CreatePlayer(Player player){
    var ch = CharacterManager.Instance.GetCharacter(ClientEntity.characterSelected.Get(player));

    var pid = player.ID;
    var r = ClientEntity.randomValue.Get(player);
    var index = ((r + 1) * (r)) % spawnPoints.Count;
    var item = spawnPoints[index];
    spawnPoints.RemoveAt(index);

    var obj = Instantiate(playerPrefab, item.position, Quaternion.identity);
    obj.name = ch.Name;

    var pc = obj.GetComponent<PlayerController>();
    pc.nva = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
    pc.nva.avoidancePriority = pid == ClientEntity.localPlayer.ID ? 50 : 0;
    pc.EntityID = pid;
    pc.authorityID = pid;

    var sr = obj.GetComponentInChildren<SpriteRenderer>();
    
    sr.sprite = ch.FullSprite;

    r *= 2;
    var gunindex = ((r + 1) * r) % GunManager.Instance.gunPrefabs.Length;
    var gunPref = GunManager.Instance.getGun(gunindex);

    var gunObj = Instantiate(gunPref, pc.handTransform);
    gunObj.name = gunPref.name;
    gunObj.transform.localPosition = new Vector3(0f, 0f, 0f);
    gunObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f);

    var gun = gunObj.GetComponent<Gun>();
    gun.EntityID = 100 + pid;
  }

}
