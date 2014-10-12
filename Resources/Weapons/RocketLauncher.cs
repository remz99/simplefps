/*
 * RocketLauncher
 * 
 */
using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon
{ 
  public override void Awake()
  {
    defaultAmmoAmount = 10;
    ammo = defaultAmmoAmount;
    fireTime = 2f;
    bulletClass = "Rocket";
    name = "RocketLauncher";
    
    base.Awake();
  }
  
  void Start()
  {
    nextFireTime = Time.time;
  }

  public override void Fire()
  {
    if (ammo > 0 && Time.time >= nextFireTime) {
      GameObject bullet = (GameObject) PhotonNetwork.Instantiate (bulletClass, BulletPosition(), camera.transform.rotation, 0); 

      bullet.rigidbody.AddForce(camera.transform.forward * 30.0f, ForceMode.Impulse);
      ammo -= 1;
      nextFireTime = Time.time + fireTime;

      transform.parent.transform.parent.GetComponent<PlayerMovement>().AddImpact(-camera.transform.forward, 55.0f);
    }
  }
}
