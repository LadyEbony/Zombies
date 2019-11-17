using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Texel
[ExecuteInEditMode]
public class FaceCameraTrue : MonoBehaviour {

  public float yoffset;

  void LateUpdate() {
    var activeCam = Camera.main;

    if (activeCam) { // Ignore if the camera is null (No camera assigned)
      transform.LookAt(activeCam.transform.position, -Vector3.up);
    }
  }
}
