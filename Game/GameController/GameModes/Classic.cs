/**
 * Classic
 *
 * Game rules for the Classic game mode. This mode is based loosely on the 
 * team buddies style game play where players must place boxes on a building
 * pad to build/spawn weapons, items and players. When a player dies they
 * change to an available/controlable team mate if not its game over.
 */
using UnityEngine;
using System.Collections;

public class Classic : GameMode
{
  void Awake()
  {
    name = "Classic";
  }

  /**
   * For this game mode players should only be respawned if there are available 
   * players to change to.
   */
  public override void PlayerDied(PlayerManager playerManager, PhotonPlayer killer)
  {
    ScoreBoard.instance.GetComponent<PhotonView>().RPC("AddKillToScoreBoard", PhotonTargets.All, killer);

    MenuCamera.instance.gameObject.SetActive(true);

    playerManager.SetNextSpawnTime();
  }
}
