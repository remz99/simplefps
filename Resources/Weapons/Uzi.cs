/*
 * Uzi
 * 
 * This weapon is a low priority weapon with a fast firing time and low damage output
 */
using UnityEngine;
using System.Collections;

public class Uzi : Weapon
{
  public override void Awake()
  {
    defaultAmmoAmount = 50;
    ammo = defaultAmmoAmount;
    fireTime = 0f;
    bulletClass = "UziBullet";
    name = "Uzi";

    base.Awake();
  }

  void Start()
  {
    nextFireTime = Time.time;
  }
}
