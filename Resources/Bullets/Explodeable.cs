/*
 * Explodeable
 * 
 * Base class for Explodeable bullets(grenade, rocket). Explodeable bullets deal damage
 * based on how close objects are to the explosion
 */
using UnityEngine;
using System.Collections;

public class Explodeable : Bullet
{
  protected float explosionForce;
  protected float explosionRadius;
  protected float upwardsModifier;

  // apply damage to damageable objects in the radius
  protected void DamageObjectsInRadius()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
    
    foreach (Collider col in hitColliders) {
      if (col.rigidbody) {
        col.rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
      }

      IDamageable damageable = (IDamageable) col.gameObject.GetComponent(typeof(IDamageable));

      if (damageable != null) {
        float distance = Vector3.Distance(transform.position, col.transform.position);
        int blastDamage = (int) (damage / distance);
        
        col.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, blastDamage, col.GetComponent<PhotonView>().owner, PhotonNetwork.player);
      }
    }
  }
}
