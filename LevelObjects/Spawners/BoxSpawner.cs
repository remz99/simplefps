/**
 * BoxSpawner
 * 
 * Spawn a box for the Classic game mode. We set the spawnPosition vector so that it looks
 * like the boxes are falling from the sky. MaxBoxes is exposed so that we can change
 * the number of boxes to spawn from within Unity inspector.
 */
using UnityEngine;
using System.Collections;

public class BoxSpawner : Spawner
{
  public int MaxBoxes = 8;

  private int count = 0;

  public static BoxSpawner instance { get; private set; }
  
  void Awake()
  {
    base.Awake();

    instance = this;
    spawnTime = 1.0f;
    spawnPosition = new Vector3 (transform.position.x, 25, transform.position.z);
  }

  public override void Start()
  {
    base.Start();
  }
  
  public override void Update()
  {
    base.Update();
  }
  
  protected override void Spawn()
  {
    if (count <= MaxBoxes) {
      PhotonNetwork.Instantiate ("Box", spawnPosition, Random.rotation, 0);
      count += 1;
    }

    nextSpawnTime = Time.time + spawnTime;
  }

  [RPC]
  public void IncrementCurrentBoxCount()
  {
    count += 1;
  }
  
  [RPC]
  public void DecrementCurrentBoxCount()
  {
    count -= 1;
  }
}
