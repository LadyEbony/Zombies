using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour {

  public static GunManager Instance;

  public GameObject[] gunPrefabs;
  public GameObject getGun(int index) => gunPrefabs[index];

  private void Awake() {
    Instance = this;
  }
}
