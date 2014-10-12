/*
 * Health
 * 
 * Add 200 health to the player
 * 
 */
using UnityEngine;
using System.Collections;

public class MegaHealth : Health
{	
  public override void Awake()
  {
    amount = 200;
	
	  base.Awake();
  }

  public override void Update()
  {
    base.Update();
  }
}
