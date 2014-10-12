/**
 * PlayerManager
 *
 * A class to manage the spawning of players which includes when to spawn
 * and where to spawn.
 */
using UnityEngine;
using System.Collections;

public class PlayerManager
{
  private bool  spawnPlayer;
  private float nextSpawnTime;
  
  private GameController gameController;
  
  private const float DEATH_TIMEOUT = 5.0f;

  public PlayerManager(GameController controller)
  {
    gameController = controller;
    gameController.RegisterUpdateDelegate(CheckForSpawn);
    spawnPlayer = true;
  }

  /**
   * Set the next time for when the player should be spanwed
   */
  public void SetNextSpawnTime()
  {
    nextSpawnTime = Time.time + DEATH_TIMEOUT;
    spawnPlayer = true;
  }
  
  /**
   * Spawn the player if the game time is greater than nextSpawnTime
   */
  public void CheckForSpawn()
  {
    if (spawnPlayer && Time.time >= nextSpawnTime) {
      SpawnPlayer();
    }
  }

  /**
   * Spawn the player
   */
  public void SpawnPlayer()
  {
    spawnPlayer = false;
    
    GameObject player = (GameObject) PhotonNetwork.Instantiate("player", SpawnPosition(), Quaternion.identity, 0);
    
    player.GetComponent<PlayerController>().Enable();
  }
  
  /**
   * Vector3 for where the player should be respawned
   */
  private Vector3 SpawnPosition()
  {
    if (BaseManager.instance) {
      return BaseManager.instance.BaseForID(PhotonNetwork.player.ID).PlayerSpawnLocation();
    } else {
      return PlayerSpawnManager.instance.newSpawnPosition();
    }
  }
}
