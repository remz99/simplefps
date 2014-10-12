/*
 * Grenade
 * 
 */
using UnityEngine;
using System.Collections;

public class Grenade : Explodeable
{
  public override void Awake()
  {
    damage = 100;
    lifespan = 2.0f;
    explosionForce = 200f;
    explosionRadius = 5.0f;
    upwardsModifier = 10.0f;
  }
  
  public override void Update()
  {
    base.Update();
  }

  void OnTriggerEnter(Collider other)
  {
    // Override the trigger callback
  }

  protected override void Explode()
  {
    // explosion effect
    Instantiate (Resources.Load("Explosions/Grenade"), transform.position, transform.rotation);

    // Damage objects in explosion raidus
    DamageObjectsInRadius();

    // destroy game object
    base.Explode();
  }
}
