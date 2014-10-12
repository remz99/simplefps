/*
 * GatlingGun
 * 
 * High priority weapon, with fast firing time and low damage
 * => automatic which means it can fire by holding down the fire button
 */
using UnityEngine;
using System.Collections;

public class GatlingGun : Weapon
{ 
  public override void Awake()
  {
    defaultAmmoAmount = 50;
    ammo = defaultAmmoAmount;
    fireTime = 0.15f;
    automatic = true;
    bulletClass = "GatlingGunBullet";
    name = "GatlingGun";

    base.Awake();
  }

  void Start()
  {
    nextFireTime = Time.time;
  }
}
