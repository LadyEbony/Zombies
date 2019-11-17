using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : EntityBase {

  [Header("Basic")]
  public int damage = 10;
  public float bulletInterval = 0.1f;
  public float bulletSpread = 2f;
  public int bulletCount = 1;
  public float bulletSpeed;

  [Header("Ammo")]
  public int clipCount = 30;
  public int clipCountMax = 30;
  public float reload = 1.5f;
  public int ammo = 90;

  [Header("Helper")]
  public bool gunReady = true;
  public bool bulletReady = true;
  private float bulletRealTime;
  private float gunRealTime;

  [Header("Audio")]
  public AudioSource audioSource;
  public AudioClip gunshotSound;

  [Header("Transforms")]
  private PlayerController pc;
  public GameObject bulletPrefab;
  public Transform muzzleTransform;

  private void Start() {
    pc = GetComponentInParent<PlayerController>();

    this.Register();

    if (pc.isMine){
      GunDisplay.Instance.UpdateText(this);
    }
  }

  // Update is called once per frame
  void Update() {
    if (!bulletReady && Time.time >= bulletRealTime) bulletReady = true;
    if (!gunReady && Time.time >= gunRealTime) { 
      gunReady = true;
      clipCount = Mathf.Min(clipCountMax, ammo);
      ammo -= clipCount;
    }
  }

  public void Fire(){
    if (gunReady && bulletReady){
      var source = muzzleTransform.position;

      for(var i = 0; i < bulletCount; i++){
        var direction = (Quaternion.LookRotation(muzzleTransform.forward, Vector3.up) * Quaternion.Euler(0f, Random.Range(-bulletSpread, bulletSpread), 0f)) * Vector3.forward;
        var hitcheck = Physics.Raycast(source, direction, out var hit, 10f, LayerMask.GetMask("Default"));
        Vector3 destination;
        if (hitcheck){
          destination = hit.point;
        } else {
          destination = source + direction * 10f;
        }

        RaiseEvent('f', true, destination);
        //FireBullet(destination);
      }

      //RaiseEvent('a', true, 0);

      bulletReady = false;
      bulletRealTime = Time.time + bulletInterval;

      clipCount--;
      if (clipCount <= 0){
        gunReady = false;
        gunRealTime = Time.time + reload;
      }

      GunDisplay.Instance.UpdateText(this);
    }
  }

  [NetEvent('f')]
  public void FireBullet(Vector3 destination){
    var source = muzzleTransform.position;
    var direction = Vector3.Normalize(destination - source);

    var comp = Instantiate(bulletPrefab, source, Quaternion.LookRotation(source, Vector3.up)).GetComponent<Bullet>();
    comp.pc = pc;
    comp.damage = damage;
    comp.speed = bulletSpeed;
    comp.destination = destination;

    // Play gunshot sound
    audioSource.PlayOneShot(gunshotSound);
  }

}
