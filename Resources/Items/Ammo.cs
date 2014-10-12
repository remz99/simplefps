/*
 * Ammo
 * 
 * Ammo items add ammo to the current player when picked up, this is done with an RPC call to the player
 * => currently it only adds ammo to the current weapon, ideally in future we can specify ammo types
 */
using UnityEngine;
using System.Collections;

public class Ammo : Item
{
  private int amount = 15;

  public override void Update()
  {
    base.Update();
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Player") {
      PickedUp(other.gameObject.GetComponent<PlayerController>());
    }
  }

  protected override void PickedUp (PlayerController player)
  {
    player.GetComponent<PhotonView>().RPC("AddAmmo", PhotonTargets.All, 15, player.GetComponent<PhotonView>().owner);

    base.PickedUp (player);
  }
}
