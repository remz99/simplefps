/*
 * Rocket
 * 
 */
using UnityEngine;
using System.Collections;

public class Rocket : Explodeable
{
  public override void Awake()
  {
    damage = 90;
    lifespan = 3.0f;
    explosionForce = 500f;
    explosionRadius = 5.0f;
    upwardsModifier = 10.0f;
  }
  
  public override void Update()
  {
    base.Update();
  }

  void OnTriggerEnter(Collider other)
  {
    Explode();
  }
  
  protected override void Explode()
  {
    // explosion effect
    Instantiate (Resources.Load("Explosions/Rocket"), transform.position, transform.rotation);

    // Damage objects in explosion raidus
    DamageObjectsInRadius();  

    // destroy game object
    base.Explode();
  }
}
