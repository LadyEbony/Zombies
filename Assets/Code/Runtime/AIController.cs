using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;

using UnityEngine.AI;

public class AIController : EntityController, EntityNetwork.IMasterOwnsUnclaimed {

  [Header("Networked")]
  public Vector3 destination;

  [Header("Additional")]
  public float aiInterval = 0.25f;
  private float aiTime;
  public float attackRange;
  public float attackInterval = 1f;
  private float attackTime;

  void OnEnable() {
    ZombieSpawner.total++;
  }

  void OnDisable() {
    ZombieSpawner.total--;
  }


  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', destination);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)) {
      destination = (Vector3)val;
    }
  }

  protected override void Update() {
    base.Update();

    var local = PlayerController.Local;
    if (local && local.isMine && Time.time >= attackTime) {
      if (Vector3.SqrMagnitude(local.transform.position - transform.position) <= attackRange * attackRange) {
        local.RaiseEvent('d', true, 1);

        attackTime = Time.time + attackInterval;
      }
    }

    if (health <= 0){
      gameObject.SetActive(false);
      Destroy(gameObject, 5f);
    }
  }

  protected override void LocalUpdate() {
    if (Time.time >= aiTime){
      nva.ResetPath();

      var list = PlayerController.GlobalList.OrderBy(p => Vector3.SqrMagnitude(p.transform.position - transform.position));
      if (list.Count() == 0) return;

      var closest = list.ElementAt(0);

      destination = closest.transform.position;
      SetPath();

      aiTime = Time.time + aiInterval;
    }
  }

  protected override void RemoteUpdate() {
    SetPath();
  }

  void SetPath(){
    NavMeshPath path = new NavMeshPath();
    var completePath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
    if (completePath && path.status == NavMeshPathStatus.PathComplete) {
      nva.SetPath(path);
    }
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.green;
    if (nva.hasPath) {
      var path = nva.path.corners;
      for (var i = 0; i < path.Length - 1; i++){
        Gizmos.DrawLine(path[i], path[i + 1]);
      }
    }
  }

}
