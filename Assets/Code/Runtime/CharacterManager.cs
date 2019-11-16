using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Character {
  public string Name;

  public Sprite HeadSprite;
  public Sprite FullSprite;

  [Header("Colors")]
  public Color OutlineColor;
  public Color InnerColor;

}

public class CharacterManager : MonoBehaviour {
  public static CharacterManager Instance { get; private set; }

  [SerializeField] private Character[] Characters;

  public Character[] GetCharacters => Characters;
  public Character GetCharacter(int i) => Characters[i];
  public Character GetRandomCharacter => Characters[GetRandomCharacterIndex];
  public int GetRandomCharacterIndex => Random.Range(0, Characters.Length);

  private void Awake() {
    if (Instance) { 
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  [ContextMenu("Add Front")]
  private void AddFrontCharacter() {
    var c = new Character[Characters.Length + 1];
    System.Array.Copy(Characters, 0, c, 1, Characters.Length);

    if (Characters.Length >= 1){
      c[0] = Characters[0];
    }

    Characters = c;
  }

  [ContextMenu("Pop Front")]
  private void PopFrontCharacter() {
    if (Characters.Length > 1) {
      var c = new Character[Characters.Length - 1];
      System.Array.Copy(Characters, 1, c, 0, c.Length);
      Characters = c;
    } else {
      Characters = new Character[0];
    }

  }

}
