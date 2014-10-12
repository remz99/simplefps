/*
 * UziBullet
 * 
 */
using UnityEngine;
using System.Collections;

public class UziBullet : Bullet
{
  public override void Awake()
  {
    damage = 5;
    lifespan = 1.0f;
  }

  public override void Update()
  {
    base.Update();
  }
}
