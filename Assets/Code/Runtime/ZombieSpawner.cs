using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ZombieSpawner : EntityBase, EntityNetwork.IMasterOwnsUnclaimed {

  public static int counter = 1100;

  public GameObject zombie;
  public Sprite[] sprites;
  public float spawnInterval;
  private float spawnTime;

  public float range;

  private IEnumerator Start() {
    while (!PlayerSpawner.gameReady) yield return null;
    this.Register();
  }

  // Update is called once per frame
  void Update() {
    if (!PlayerSpawner.gameReady || isRemote) return;

    if (Time.time >= spawnTime){
      var ran = Random.insideUnitSphere * range;

      var status = NavMesh.SamplePosition(transform.position + ran, out var hit, range * 2f, NavMesh.AllAreas);
      if (!status){
        Debug.Log("Could not find spawn point");
        return;
      }

      RaiseEvent('z', true, counter++, hit.position);
      spawnTime = Time.time + spawnInterval;
    }
  }

  [NetEvent('z')]
  public void SpawnZombie(int id, Vector3 position) {
    var obj = Instantiate(zombie, position, Quaternion.identity);

    obj.GetComponent<AIController>().EntityID = id;
    obj.GetComponentInChildren<SpriteRenderer>().sprite = sprites[id % sprites.Length];
  }


  private void OnDrawGizmos() {
    Gizmos.color = Color.red;
    GizmoExtender.DrawWireCircle(transform.position, range);
  }
}
