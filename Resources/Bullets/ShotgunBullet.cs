/*
 * ShotgunBullet
 * 
 */
using UnityEngine;
using System.Collections;

public class ShotgunBullet : Bullet
{
  public override void Awake()
  {
    damage = 15;
    lifespan = 0.25f;
  }
  
  public override void Update()
  {
    base.Update();
  }
}