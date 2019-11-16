using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;

using EntityNetwork;

public class LevelLoader : EntityBase, IAutoRegister {

  private static LevelLoader _instance;
  public static LevelLoader instance {
    get {
      return _instance;
    }
  }

  public override void Awake(){
    base.Awake();

    if (_instance == null) {
      _instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }

  }

  public void RaiseStartGame(int scene){
    RaiseEvent('L', true, scene);
  }

  [NetEvent('L')]
  void loadEvent(int scene) {
    if (SceneManager.GetActiveScene().buildIndex == scene) return;

    StartCoroutine(AsyncSceneLoad(scene));
  }

  public float progress = 0.0f;

  IEnumerator AsyncSceneLoad(int scene){
    progress = 0.0f;
    var asyncLoad = SceneManager.LoadSceneAsync(scene);
    asyncLoad.allowSceneActivation = false;
    
    while (asyncLoad.progress < 0.9f){
      yield return null;
      progress = asyncLoad.progress;
    }

    if (!Application.isEditor){
      //yield return new WaitForSeconds(10);
    }

    asyncLoad.allowSceneActivation = true;
  }


}