/**
 * Base
 * 
 * A Base encompasses all the objects for a team/player
 * Each base has:
 * BaseHQ => the primary building for a base which can be destroyed/damaged
 * SpawnLocation => the vector to be used as the position where players belonging to this
 *             base/team are spawned at
 * BuildingPad => Place to build boxes / spawn items from
 */
using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour
{
  public int ID;

  public Vector3 PlayerSpawnLocation()
  {
    return transform.FindChild("SpawnLocation").transform.position;
  }

  public void Explode()
  {
    PlayerController[] players = FindObjectsOfType (typeof(PlayerController)) as PlayerController[];

    foreach (PlayerController player in players) {
      if (player.teamID == ID) {
        if (player.GetComponent<PhotonView>().owner == PhotonNetwork.player) {
          player.GetComponent<PhotonView>().RPC("EndGame", PhotonTargets.All, PhotonNetwork.player);
        }
      }
    }
  }
}
