/*
 * Item
 * 
 * Base class for items, items are objects which players can pick up for example health and ammo.
 * => for each Item class there is a corresponding Spawner class which spawns the item.
 * => when an item is picked up we send an RPC to tell the Spawner that it can respawn an item
 * 
 */
using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{ 
  public virtual void Awake()
  {
  }

  public virtual void Update()
  {
    Rotate();
  }

  void Rotate()
  {
    transform.Rotate(Vector3.down * Time.deltaTime * 75);
  }

  protected virtual void PickedUp(PlayerController player)
  {
    // send RPC to spawner to respawn item
    if (transform.parent) {
      transform.parent.GetComponent<PhotonView>().RPC ("PickedUp", PhotonTargets.All, null);
   }
      
    // send RPC to master client to destory this item
    player.GetComponent<PhotonView>().RPC("DestroyObject", PhotonTargets.MasterClient, GetComponent<PhotonView>().viewID);
  }

  [RPC]
  public void SetParent(int viewID)
  {
    transform.parent = PhotonView.Find(viewID).transform;
  }
}
