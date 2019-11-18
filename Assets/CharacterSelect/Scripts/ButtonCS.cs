using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ButtonCS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

  private Image outlineImage, innerImage, characterImage;

  private Color baseColor;

  public bool hovered;
  public int index;

  void Awake(){
    innerImage = transform.Find("Backdrop").GetComponent<Image>();
    outlineImage = transform.Find("Outline").GetComponent<Image>();
    characterImage = transform.Find("Character").GetComponent<Image>();

    innerImage.material = new Material(innerImage.material);
  }

  void Update(){
    if (innerImage) {
      innerImage.material.SetColor("_Color", hovered ? new Color(0.75f, 0.75f, 0.75f, 1f) : Color.white);
    }
  }

  public void SetCharacter(Character c, int i){
    var character = c;
    index = i;

    outlineImage.color = c.OutlineColor;
    innerImage.color = c.InnerColor;
    characterImage.sprite = c.HeadSprite;
  }

  public void OnPointerEnter(PointerEventData eventData) {
    ClientEntity.characterHovered.SetLocal(index);
    hovered = true;
  }

  public void OnPointerExit(PointerEventData eventData) {
    hovered = false;
  }

  public void OnPointerClick(PointerEventData eventData) {
    ClientEntity.characterSelected.SetLocal(index);
    ClientEntity.lobbyStatus.SetLocal(true);
    ClientEntity.randomValue.SetLocal(Random.Range(0, 256));
  }
}
