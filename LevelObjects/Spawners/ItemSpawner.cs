/**
 * ItemSpawn
 * 
 * Spawn a set weapon, used in both game modes
 */
using UnityEngine;
using System.Collections;

public class ItemSpawner : Spawner
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
