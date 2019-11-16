using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

  public int damage;
  public float speed;
  public Vector3 destination;

  // Update is called once per frame
  void Update(){
    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    if (Vector3.SqrMagnitude(transform.position - destination) < 0.125f){
      Destroy(gameObject);
    }
  }
}
