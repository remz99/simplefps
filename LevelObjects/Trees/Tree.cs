/**
 * Tree
 * 
 * Trees are level objects which can take damage and when destroyed spawn either health or ammo
 */
using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour, IDamageable {

  private int  health = 50;
  private bool alive  = true;
  private Base teamBase;
  private Color color;

  void Start()
  {
    teamBase = transform.parent.GetComponent<Base>();
  }
  
  [RPC]
  public void TakeDamage(int damage, PhotonPlayer owner, PhotonPlayer killer)
  {
    if (alive) {
      if (health > 0) {
        health -= damage;
      }

      if (health <= 0) {
        alive = false;

        if (PhotonNetwork.isMasterClient) {
          SpawnItem();
        }

        GetComponent<PhotonView>().RPC("Destroy", PhotonTargets.All, null);
      }
    }
  }

  /**
   * Randomly spawn an item(health or ammo)
   */
  private void SpawnItem()
  {
    if (PhotonNetwork.isMasterClient) {
      if (Random.Range(1, 10) >= 6) {
        PhotonNetwork.Instantiate ("Ammo", ItemSpawnPosition(), Quaternion.identity, 0);
      } else {
        PhotonNetwork.Instantiate ("Health", ItemSpawnPosition(), Quaternion.identity, 0);
      }
    }
  }

  /**
   *  Return the vector for new spawed item
   */
  private Vector3 ItemSpawnPosition()
  {
    return transform.localPosition + new Vector3(0f, 1f, 0f);
  }

  /**
   * Instead of destroying the tree over the network just disable
   * it for each client
   */
  [RPC]
  public void Destroy() 
  {
    gameObject.SetActive(false);
  }
}
