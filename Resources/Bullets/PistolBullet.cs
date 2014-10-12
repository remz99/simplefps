/*
 * PistolBullet
 * 
 */
using UnityEngine;
using System.Collections;

public class PistolBullet : Bullet
{ 
  public override void Awake()
  {
    damage = 10;
    lifespan = 1.0f;
  }

  public override void Update()
  {
    base.Update();
  }
}
