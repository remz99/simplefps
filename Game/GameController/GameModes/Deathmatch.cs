/**
 * Deathmatch
 *
 * Game rules for the Deathmatch game mode. Deathmatch is a free for all
 * game mode with weapons and items already on the map which respawned when
 * picked up. On player deaths the player will respawn.
 */
using UnityEngine;
using System.Collections;

public class Deathmatch : GameMode
{
  string prefabName = "AI/DeathmatchBot";

  void Awake()
  {
    name = "DeathMatch";
  }

  public override void LoadBots()
  {
    PhotonNetwork.Instantiate(prefabName, PlayerSpawnManager.instance.newSpawnPosition(), Quaternion.identity, 0);
  }
}
