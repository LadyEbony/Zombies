﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

  public GameObject deathGraphic;

  public PlayerController pc;

  public int damage;
  public float speed;
  public Vector3 destination;

  // Update is called once per frame
  void Update(){
    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    if (Vector3.SqrMagnitude(transform.position - destination) < 0.125f){
      CreateDeathGraphic(destination);
      Destroy(gameObject);
    }
  }

  private void OnTriggerEnter(Collider col) {
    if (pc.isMine) {
      var ai = col.transform.gameObject.GetComponent<AIController>();
      ai.RaiseEvent('d', true, damage);
    }

    CreateDeathGraphic(transform.position);
  }

  void CreateDeathGraphic(Vector3 position){
    var obj = Instantiate(deathGraphic, position, Quaternion.identity);
    Destroy(obj, 0.1f);
  }
}
