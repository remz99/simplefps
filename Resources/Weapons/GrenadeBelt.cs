/*
 * GrenadeBelt
 * 
 * Fires "Grenade" bullet types which applys gravity to achieve a lobbing effect
 */
using UnityEngine;
using System.Collections;

public class GrenadeBelt : Weapon
{
  public override void Awake()
  {
    defaultAmmoAmount = 5;
    ammo = defaultAmmoAmount;
    fireTime = 1.5f;
    bulletClass = "Grenade";
    name = "GrenadeBelt";
    
    base.Awake();
  }
  
  void Start()
  {
    nextFireTime = Time.time;
  }

  public override void Fire()
  {
    if (ammo > 0 && Time.time >= nextFireTime) {
      GameObject bullet = (GameObject) PhotonNetwork.Instantiate (bulletClass, BulletPosition(), Quaternion.identity, 0); 

      // grenade bullets have gravity enabled
      bullet.rigidbody.AddForce(camera.transform.forward * 7.5f, ForceMode.Impulse);

      // we want to lob grenades so add some upwards force
      bullet.rigidbody.AddForce(Vector3.up * 7.5f, ForceMode.Impulse);

      ammo -= 1;
      nextFireTime = Time.time + fireTime;
    }
  }
}
