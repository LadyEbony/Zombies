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

  protected override void LocalUpdate() {
    if (Time.time >= aiTime){
      nva.ResetPath();

      var closest = PlayerController.GlobalList.OrderBy(p => Vector3.SqrMagnitude(p.transform.position - transform.position)).ElementAt(0);

      destination = closest.transform.position;
      nva.destination = destination;

      aiTime = Time.time + aiInterval;
    }
  }

  protected override void RemoteUpdate() {
    nva.destination = destination;
  }

  [NetEvent('d')]
  public void TakeDamage(int damage){

  }

}
