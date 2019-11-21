using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

using UnityEngine.AI;

public class PlayerController : EntityController, EntityNetwork.IMasterOwnsUnclaimed {

  public static PlayerController Local;
  public static List<PlayerController> GlobalList;

  static PlayerController(){
    GlobalList = new List<PlayerController>();
  }

  [Header("Networked Variables")]
  public Vector3 position;
  public float rotation;
  public int kills;

  [Header("Audio")]
  public AudioTimer footstepAudioPlayer;

  [Header("Additional")]
  public Gun gunInHand;
  public Transform handTransform;
  public Transform healthTransform;
  public GameObject reloadDisplay;
  public Transform reloadTransform;

  void OnEnable(){
    GlobalList.Add(this);
  }

  void OnDisable(){
    GlobalList.Remove(this);
  }

  protected override void StartProcedure() {
    base.StartProcedure();
    position = transform.position;

    gunInHand = GetComponentInChildren<Gun>();
  }

  protected override void StartNetworkProcedure() {
    base.StartNetworkProcedure();

    if (isMine) Local = this;
    Score.Instance.Add(this);
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
    h.Add('r', rotation);
    h.Add('k', kills);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    object val;
    if (h.TryGetValue('p', out val)) {
      position = (Vector3)val;
    }

    if (h.TryGetValue('r', out val)) {
      rotation = (float)val;
    }

    if (h.TryGetValue('k', out val)){
      kills = (int)val;
    }
  }

  protected override void Update() {
    base.Update();

    footstepAudioPlayer.Playing = velocity.sqrMagnitude > 0;

    healthTransform.localScale = new Vector3((float)health / healthMax, 1f, 1f);

    if (health <= 0){
      Score.Instance.Remove(this);

      gameObject.SetActive(false);
      Destroy(gameObject, 5f);
    }
  }


  protected override void LocalUpdate(){
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

  protected override void RemoteUpdate(){
    nva.destination = position;
    handTransform.rotation = Quaternion.Slerp(handTransform.rotation, Quaternion.Euler(0f, rotation, 0f), 0.5f);
  }

}
