using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class GunDisplay : MonoBehaviour {
  public static GunDisplay Instance;

  private TextMeshProUGUI textMesh;

  private void Awake() {
    Instance = this;
    textMesh = GetComponent<TextMeshProUGUI>();
  }

  public void UpdateText(Gun gun){
    textMesh.text = string.Format("{0}/{1}   [{2}]", gun.clipCount, gun.clipCountMax, gun.ammo);
  }
}
