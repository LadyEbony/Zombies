using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCS : MonoBehaviour {

  public Character character;
  public int index;

  public void SetCharacter(Character c, int i){
    Transform t;
    character = c;
    index = i;

    t = transform.Find("Outline");
    t.GetComponent<Image>().color = c.OutlineColor;

    t = transform.Find("Backdrop");
    t.GetComponent<Image>().color = c.InnerColor;

    t = transform.Find("Character");
    t.GetComponent<Image>().sprite = c.HeadSprite;

    var b = GetComponent<Button>();
    b.onClick.RemoveAllListeners();
    b.onClick.AddListener(() => SelectCharacter(index));
  }

  public void SelectCharacter(int i){
    ClientEntity.SetCharacter(i);
    ClientEntity.SetReadyStatus(true);
  }

}
