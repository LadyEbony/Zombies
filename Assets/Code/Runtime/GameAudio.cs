using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour {

  // Update is called once per frame
  void Update(){
    if (!PlayerSpawner.gameReady) return;

    if (PlayerController.Local == null || !PlayerController.Local.gameObject.activeSelf){
      Destroy(gameObject);
    } 
  }
}
