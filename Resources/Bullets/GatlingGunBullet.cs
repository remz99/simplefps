/*
 * GatlingGunBullet
 * 
 */
using UnityEngine;
using System.Collections;

public class GatlingGunBullet : Bullet
{
  public override void Awake()
  {
    damage = 3;
    lifespan = 1.0f;
  }
  
  public override void Update()
  {
    base.Update();
  }
}
