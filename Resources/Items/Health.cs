/*
 * Health
 * 
 * Health items add health to the current player, this is done with an RPC call to the player
 * 
 */
using UnityEngine;
using System.Collections;

public class Health : Item
{
  protected int amount = 25;

  public override void Awake()
  {
    base.Awake();
  }

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
    player.GetComponent<PhotonView>().RPC("AddHealth", PhotonTargets.All, amount, player.GetComponent<PhotonView>().owner);

    base.PickedUp(player);
  }
}
