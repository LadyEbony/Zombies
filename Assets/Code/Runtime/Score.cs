using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Score : MonoBehaviour {

  public static Score Instance;

  public GameObject local;
  public GameObject client;

  public Dictionary<PlayerController, TextMeshProUGUI> set;

  private void Awake() {
    Instance = this;

    set = new Dictionary<PlayerController, TextMeshProUGUI>();

    client.SetActive(false);
    client.transform.parent = null;
  }

  public void Add(PlayerController pc){
    if (pc.isMine){
      set.Add(pc, local.GetComponent<TextMeshProUGUI>());
    } else {
      var copy = Instantiate(client, transform);
      copy.SetActive(true);
      set.Add(pc, copy.GetComponent<TextMeshProUGUI>());
    }
  }

  public void Remove(PlayerController pc){
    TextMeshProUGUI item;
    if (set.TryGetValue(pc, out item)){
      Destroy(item.gameObject);
    }
    set.Remove(pc);
  }

  // Update is called once per frame
  void Update(){
    foreach(var item in set){
      var pc = item.Key;
      item.Value.text = string.Format("{0} <color=#ff0000>HP</color> {1} <color=#00ff00>KILLS</color> {2}", pc.name, pc.health, pc.kills);
    }
  }
}
