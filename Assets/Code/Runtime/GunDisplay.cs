using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GunDisplay : MonoBehaviour {

  public static GunDisplay ClipInstance, AmmoInstance, GunInstance;

  public TextMeshProUGUI textMesh;

  private void Awake() {

    switch (gameObject.name) {
      case "Gun":
        GunInstance = this;
        break;
      case "Clip":
        ClipInstance = this;
        break;
      case "Ammo":
        AmmoInstance = this;
        break;
    }

    textMesh = GetComponent<TextMeshProUGUI>();
  }
}

