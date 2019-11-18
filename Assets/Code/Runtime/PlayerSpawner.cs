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

    ClientEntity.gameStatus.SetLocal(true);

    while (!NetworkManager.inGamePlayersReady) yield return null;

    if (NetworkManager.inRoom) {
      var players = NetworkManager.getSortedPlayers;
      foreach (var p in players){
        var pid = p.ID;
        var r = ClientEntity.randomValue.Get(p);
        var index = ((r + 1) * (r)) % spawnPoints.Count;
        var item = spawnPoints[index];
        spawnPoints.RemoveAt(index);

        var obj = Instantiate(playerPrefab, item.position, Quaternion.identity);
        var pc = obj.GetComponent<PlayerController>();
        pc.nva = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
        pc.nva.avoidancePriority = p.ID == ClientEntity.localPlayer.ID ? 50 : 0;
        pc.EntityID = pid;
        pc.authorityID = pid;

        var sr = obj.GetComponentInChildren<SpriteRenderer>();
        var ch = CharacterManager.Instance.GetCharacter(ClientEntity.characterSelected.Get(p));
        sr.sprite = ch.FullSprite;

        r *= 2;
        var gunindex = ((r + 1) * r) % GunManager.Instance.gunPrefabs.Length;
        var gunPref = GunManager.Instance.getGun(gunindex);

        var gunObj = Instantiate(gunPref, pc.handTransform);
        gunObj.transform.localPosition = new Vector3(0f, 0f, 0f);
        gunObj.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f);

        var gun = gunObj.GetComponent<Gun>();
        gun.EntityID = 100 + pid;
      }
    }

    gameReady = true;

  }

}
