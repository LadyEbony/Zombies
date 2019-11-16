using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

using UnityEngine.AI;

public class PlayerController : EntityBase, EntityNetwork.IMasterOwnsUnclaimed {

  private NavMeshAgent nva;

  public float speed => nva.speed;
  public Vector3 velocity {
    get => nva.velocity;
    set => nva.velocity = value;
  }
  public float acceleration => nva.acceleration;

  [Header("Additional")]
  public Gun gunInHand;
  public Transform handTransform;

  [Header("Network Timers")]
  public float updateInterval = 0.1f;
  private float updateTime;

  [Header("Networked Variables")]
  public Vector3 position;
  public float rotation;

  private void Start() {
    nva = GetComponent<NavMeshAgent>();
    gunInHand = GetComponentInChildren<Gun>();

    position = transform.position;

    if (!NetworkManager.inRoom) authorityID = -1;

    this.Register();
  }


  // Update is called once per frame
  void Update(){
    if (isMine){
      LocalUpdate();
    } else {
      RemoteUpdate();
      transform.localScale = Vector3.one * 0.75f;
    }

    var realtime = Time.realtimeSinceStartup;
    if (isMine && realtime > updateTime){
      UpdateNow();
      updateTime = realtime + updateInterval;
    }
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
    h.Add('r', rotation);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)){
      position = (Vector3)val;
    }

    if (h.TryGetValue('r', out val)){
      rotation = (float)val;
    }
  }

  void LocalUpdate(){
    nva.ResetPath();

    // movement
    var steering = GetDirectionInput;
    velocity = Vector3.MoveTowards(velocity, steering * speed, acceleration);

    // mouse direction
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    var hitcheck = Physics.Raycast(ray, out var hit);

    if (hitcheck){
      var hitpoint = hit.point;
      hitpoint.y = transform.position.y;
      var dir = hitpoint - transform.position;

      handTransform.rotation = Quaternion.LookRotation(dir, Vector3.up);
      rotation = handTransform.rotation.eulerAngles.y;

      Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 0.1f);
    }

    // gun
    if (Input.GetMouseButton(0))
      gunInHand?.Fire();
  }

  void RemoteUpdate(){
    nva.destination = position;

    handTransform.rotation = Quaternion.Slerp(handTransform.rotation, Quaternion.Euler(0f, rotation, 0f), 0.5f);
  }

  private Vector3 GetDirectionInput {
    get {
      var ct = Camera.main.transform;

      // Get the forward and right for the camera, flatten them on the XZ plane, then renormalize
      var fwd = ct.forward;
      var right = ct.right;

      fwd.y = 0;
      right.y = 0;

      fwd = fwd.normalized;
      right = right.normalized;

      var hor = Input.GetAxisRaw("Horizontal");
      var ver = Input.GetAxisRaw("Vertical");

      var delta = hor * right;
      delta += ver * fwd;

      // Clamp magnitude, so going diagonally isn't faster.
      if (delta != Vector3.zero) delta = delta.normalized;

      return delta;
    }
  }
}
