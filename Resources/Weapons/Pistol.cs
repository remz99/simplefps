/*
 * Pistol
 * 
 * TODO: Remove
 */
using UnityEngine;
using System.Collections;

public class Pistol : Weapon
{ 
  public override void Awake()
  {   
    defaultAmmoAmount = 15;
    ammo = defaultAmmoAmount;
    fireTime = 0.75f;
    bulletClass = "PistolBullet";
    name = "Pistol";

    base.Awake();
  }

  void Start()
  {
    nextFireTime = Time.time;
  }
}
