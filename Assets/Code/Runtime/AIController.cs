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

      if (Vector3.SqrMagnitude(closest.transform.position - transform.position) <= attackRange * attackRange){
        closest.RaiseEvent('d', true, 1);
      }

      destination = closest.transform.position;
      nva.destination = destination;

      aiTime = Time.time + aiInterval;
    }
  }

  protected override void RemoteUpdate() {
    nva.destination = destination;
  }

}
