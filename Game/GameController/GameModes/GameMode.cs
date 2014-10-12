/**
 * GameMode
 *
 * Parent class for game modes, each subclass provides rules for each
 * game mode.
 */
using UnityEngine;
using System.Collections;

public class GameMode
{
  protected string name;

  /**
   * Getter for name
   */
  public string Name()
  {
    return name;
  }

  /**
   * Return a new GameMode for the given levelID. This is used to load a 
   * game mode based on the scene ID.
   */
  public static GameMode GameModeFor(int levelID)
  {
    if (levelID == 1) {
      return new Deathmatch();
    } else {
      return new Classic();
    }
  }

  /**
   * How to handle player deaths, each subclass can set their own rules for 
   * player deaths
   */
  public virtual void PlayerDied(PlayerManager playerManager, PhotonPlayer killer)
  {
    ScoreBoard.instance.GetComponent<PhotonView>().RPC("AddKillToScoreBoard", PhotonTargets.All, killer);

    MenuCamera.instance.gameObject.SetActive(true);

    playerManager.SetNextSpawnTime();
  }

  /**
   *  List of Default weapons for each mode
   */
  public virtual string[] Weapons()
  {
    return new string[] { 
      null, 
      "Uzi",              // 1 => single box => pistol
      "GrenadeBelt",      // 2 => two boxes horizontal => uzi
      "Shotgun",          // 3 => two boxes vertical => new player / team buddie
      "RocketLauncher",   // 4 => four boxes bottom => gatling gun
      "GatlingGun",       // 5 => four boxes vertical => new player / super team buddie
      "GatlingGun",       // 6 => 4x4 boxes => tank
    };
  }

  public virtual void LoadBots()
  {
  }
}
