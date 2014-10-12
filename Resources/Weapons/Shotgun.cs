/*
 * Shotgun
 * 
 * This weapon is a medium priority weapon with a slow firing time but large damage output
 * => fires five bullets at once in a random forward direction
 * => bullets have a short lifespawn so they don't exist/travel for long
 * 
 * TODO: Redo weapon firing to be more cone like
 */
using UnityEngine;
using System.Collections;

public class Shotgun : Weapon
{ 
  public override void Awake()
  {
    defaultAmmoAmount = 10;
    ammo = defaultAmmoAmount;
    fireTime = 1.15f;
    bulletClass = "ShotgunBullet";
    name = "Shotgun";

    base.Awake();
  }

  void Start()
  {
    nextFireTime = Time.time;
  }

  public override void Fire()
  {
    if (ammo > 0 && Time.time >= nextFireTime) {
      for (int i = 0; i < 5; i++) {
        Quaternion rotation = camera.transform.rotation;
        rotation.x += Random.Range(-10.0f, 10.0f);
        rotation.y += Random.Range(-10.0f, 10.0f);

        GameObject bullet = (GameObject) PhotonNetwork.Instantiate("ShotgunBullet", BulletPosition(), rotation, 0); 
        bullet.rigidbody.AddForce(camera.transform.forward * 25.0f, ForceMode.Impulse);
      }
      ammo -= 1;
      nextFireTime = Time.time + fireTime;
    }
  }
}
