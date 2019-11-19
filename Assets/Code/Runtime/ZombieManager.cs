using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieManager : EntityBase, EntityNetwork.IMasterOwnsUnclaimed {

  public static int ZOMBIE_TOTAL = 0;
  public const int ZOMBIE_MAX = 40;

  public static List<ZombieSpawner> spawners = new List<ZombieSpawner>();

  [Header("Base")]
  public GameObject zombiePrefab;
  public Sprite[] zombieSprites;
  public int zombieCounter = 1100;

  private float startTime = 0f;
  private float nextSpawnTime;
  [Header("Difficulty over Time")]
  public int baseHealth = 100;
  public float healthPerSecond = 100f / 180f;

  public float baseSpeed = 3.5f;
  public float speedPerSecond = 1f / 180f;

  public float baseSpawnRatePerSecond = 1f;
  public float increasedSpawnRatePerSecond = 1f / 180f;

  // Start is called before the first frame update
  private IEnumerator Start() {
    this.Register();

    while (!PlayerSpawner.gameReady) yield return null;
    startTime = Time.time;
  }

  // Update is called once per frame
  void Update(){
    if (PlayerSpawner.gameReady && isMine || ZOMBIE_TOTAL < ZOMBIE_MAX) {
      if (Time.time >= nextSpawnTime){
        var time = Time.time - startTime;

        var options = new List<ZombieSpawner>();
        bool result;

        foreach(var z in spawners){
          result = true;
          foreach(var p in PlayerController.GlobalList){
            if (z.WithinRange(p.position)) {
              result = false;
              break;
            }
          }
          if (result) options.Add(z);
        }

        if (options.Count == 0) return;

        var item = options[Random.Range(0, options.Count)];
        var position = item.transform.position;
        var range = item.range;
        var randomOffset = Random.insideUnitSphere * range;

        var status = NavMesh.SamplePosition(position + randomOffset, out var hit, range * 2f, NavMesh.AllAreas);
        if (!status) {
          Debug.Log("Could not find spawn point");
          return;
        }

        RaiseEvent('z', true, zombieCounter++, hit.position, time);

        nextSpawnTime += 1f / (baseSpawnRatePerSecond + time * increasedSpawnRatePerSecond);
      }
    }
  }

  [NetEvent('z')]
  public void SpawnZombie(int id, Vector3 position, float time){
    var obj = Instantiate(zombiePrefab, position, Quaternion.identity);

    var ai = obj.GetComponent<AIController>();
    ai.EntityID = id;

    var hp = baseHealth + (int)(time * healthPerSecond);
    ai.health = hp;
    ai.healthMax = hp;

    var nva = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
    nva.speed = baseSpeed + time * speedPerSecond;

    obj.GetComponentInChildren<SpriteRenderer>().sprite = zombieSprites[id % zombieSprites.Length];
  }
}
