using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterSelectPortrait : MonoBehaviour {

  [Header("Id")]
  public int playerId;
  public bool ready;

  [HideInInspector]
  public int characterId;
  [HideInInspector]
  public Character character;

  [Header("Images")]
  public Image[] outlineImages;
  public Image[] innerImages;
  public Image portraitImage;

  [Header("Text")]
  public TextMeshProUGUI characterTextMesh;

  private void Update() {
    Player p;
    if (NetworkManager.inRoom){
      p = NetworkManager.net.CurrentRoom.GetPlayer(playerId);
      if (p == null) return;
    } else {
      p = ClientEntity.localPlayer;
    }

    ready = ClientEntity.GetReadyStatus(p);
    SetPlayer(p);
  }

  public void SetPlayer(Player player){
    playerId = player.ID;
    characterId = ClientEntity.GetCharacter(player);

    if (characterId >= 0) {
      character = CharacterManager.Instance.GetCharacter(characterId);

      foreach (var o in outlineImages) o.color = character.OutlineColor;
      foreach (var i in innerImages) i.color = character.InnerColor;
      portraitImage.sprite = character.HeadSprite;
      portraitImage.color = Color.white;

      characterTextMesh.text = character.Name;
    } else {
      foreach (var o in outlineImages) o.color = Color.black;
      foreach (var i in innerImages) i.color = Color.white;
      portraitImage.sprite = null;
      portraitImage.color = Color.clear;

      characterTextMesh.text = string.Empty;
    }
  }

}
