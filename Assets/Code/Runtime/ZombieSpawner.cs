using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour {

  public float range;

  void OnEnable(){
    ZombieManager.spawners.Add(this);
  }

  void OnDisable(){
    ZombieManager.spawners.Remove(this);
  }

  public bool WithinRange(Vector3 position) => Vector3.SqrMagnitude(position - transform.position) <= range * range;

  private void OnDrawGizmos() {
    Gizmos.color = Color.red;
    GizmoExtender.DrawWireCircle(transform.position, range);
  }
}
