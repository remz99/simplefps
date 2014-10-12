/**
 * Spawner
 * 
 * Spawners can spawn item objects for the game. With Spawners only the master client
 * can instanitate objects
 */
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
  protected bool spawn  = true;
  protected bool active = false;
  
  protected float spawnTime = 5.3f;
  protected float nextSpawnTime;
  
  protected Vector3 spawnPosition;

  public string objectName;

  public virtual void Awake()
  {
    spawnPosition = transform.position + new Vector3 (0, 1, 0);

    if (PhotonNetwork.isMasterClient) {
      active = true;
    }
  }

  public virtual void Start()
  {
    nextSpawnTime = Time.time;
  }
  
  public virtual void Update()
  {
    if (active && spawn && Time.time >= nextSpawnTime) {
      Spawn();
    }
  }

  protected virtual void Spawn()
  {
    if (active) {
      spawn = false;
      GameObject go = PhotonNetwork.Instantiate (objectName, spawnPosition, Quaternion.identity, 0);

      // RPC to set parent
      go.GetComponent<PhotonView>().RPC("SetParent", PhotonTargets.All, GetComponent<PhotonView>().viewID);
    }
  }

  [RPC]
  public virtual void PickedUp()
  {
    if (active) {
      spawn = true;
      nextSpawnTime = Time.time + spawnTime;
    }
  }
}
