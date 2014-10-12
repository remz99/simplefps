/**
 * WeaponSpawn
 * 
 * Spawn a set weapon, used in the Deathmatch game mode
 */
using UnityEngine;
using System.Collections;

public class WeaponSpawner : Spawner
{ 
  public override void Awake()
  {
    spawnTime = 5.3f;
    base.Awake();
  }

  public override void Start()
  {
    base.Start();
  }

  public override void Update()
  {
    base.Update();
  }
}
