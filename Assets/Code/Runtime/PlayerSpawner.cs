using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PlayerSpawner : MonoBehaviour {

  public static bool gameReady;

  public GameObject playerPrefab;
  private List<Transform> spawnPoints;

  public GameObject singleDebug;

  // Start is called before the first frame update
  IEnumerator Start() {
    if (NetworkManager.inRoom) Destroy(singleDebug);
    gameReady = false;

    spawnPoints = new List<Transform>();
    foreach (Transform child in transform)
      spawnPoints.Add(child);

    ClientEntity.SetSceneStatus(true);

    while (!NetworkManager.inGamePlayersReady) yield return null;

    if (NetworkManager.inRoom) {
      var players = NetworkManager.getSortedPlayers;
      foreach (var p in players){
        var pid = p.ID;
        var item = spawnPoints[0];
        spawnPoints.RemoveAt(0);

        var obj = Instantiate(playerPrefab, item.position, Quaternion.identity);
        var pc = obj.GetComponent<PlayerController>();
        pc.nva.avoidancePriority = p.ID == ClientEntity.localPlayer.ID ? 50 : 0;
        pc.EntityID = pid;
        pc.authorityID = pid;

        var sr = obj.GetComponentInChildren<SpriteRenderer>();
        var ch = CharacterManager.Instance.GetCharacter(ClientEntity.GetCharacter(p));
        sr.sprite = ch.FullSprite;

        var gun = obj.GetComponentInChildren<Gun>();
        gun.EntityID = 100 + pid;
      }
    }

    gameReady = true;

  }

}
