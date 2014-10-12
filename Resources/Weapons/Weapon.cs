/*
 * Weapon
 * 
 * This is the base weapon class which sets out variables for each weapon
 * => currently all weapons are projectile based instead of hitscan
 */
using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
  protected int defaultAmmoAmount = 10;
  protected int ammo = 10;

  protected float fireTime = 0.75f;
  protected float nextFireTime = 0.0f;

  protected bool pickedUp = false;
  protected bool automatic = false;

  protected string name;
  protected string bulletClass;

  protected Camera camera;
  
  public virtual void Awake()
  {
    camera = Camera.main;
  }

  public virtual void Fire()
  {
    if (ammo > 0 && Time.time >= nextFireTime) {
      GameObject bullet = (GameObject) PhotonNetwork.Instantiate (bulletClass, BulletPosition(), Quaternion.identity, 0); 
      bullet.rigidbody.AddForce(camera.transform.forward * 20.0f, ForceMode.Impulse);
      ammo -= 1;
      nextFireTime = Time.time + fireTime;
    }
  }

  public bool Automatic()
  {
    return automatic;
  }

  public int DefaultAmmoAmount()
  {
    return defaultAmmoAmount;
  }

  public void AddAmmo(int amount)
  {
    ammo += amount;
  }

  public int Ammo()
  {
    return ammo;
  }

  public string Name()
  {
    return name;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Player" && !pickedUp) {

      // RPC to tell the Spawner it can respawn
      if (transform.parent) {
        transform.parent.GetComponent<PhotonView>().RPC("PickedUp", PhotonTargets.All, null);
      }

      // RPC to add the weapon to the player
      other.gameObject.GetComponent<PhotonView>().RPC("AddWeapon", PhotonTargets.All, Name(), PhotonNetwork.player);
    
      // RPC to destory object for the passed owner
      other.gameObject.GetComponent<PhotonView>().RPC("DestroyObjectByOwner", PhotonTargets.All, other.GetComponent<PhotonView>().owner, GetComponent<PhotonView>().viewID);
    }
  }

  [RPC]
  public void PickUp()
  {
    pickedUp = true;
  }

  [RPC]
  public void SetParent(int viewID)
  {
    transform.parent = PhotonView.Find(viewID).transform;
  }

  [RPC]
  public void SetParent(int viewID, bool forWeapon)
  {
    PlayerController player = PhotonView.Find(viewID).GetComponent<PlayerController>();
    transform.parent = player.transform.FindChild("Camera");
  }
  
  protected Vector3 BulletPosition()
  {
    return camera.transform.position + camera.transform.forward;
  } 
}
