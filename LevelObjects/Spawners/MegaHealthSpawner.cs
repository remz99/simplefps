/**
 * MegaHealthSpawner
 * 
 * Spawn a mega health item
 */
using UnityEngine;
using System.Collections;

public class MegaHealthSpawner : Spawner
{
  public override void Awake()
  {
    spawnTime = 60.0f;
    objectName = "MegaHealth";

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
