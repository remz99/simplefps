/*
 * Bullet
 * 
 * => Each bullet class sets its own damage and lifespan.
 * => Bullets are destroyed once lifespan is <= 0
 * => For each Weapon class there is a corresponding bullet class
 */
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
  protected int damage = 1;
  protected float lifespan = 1.0f;

  public virtual void Awake() {}
  
  public virtual void Update()
  {
    UpdateLifespan();
  }

  void UpdateLifespan()
  {
    lifespan -= Time.deltaTime;
    
    if (lifespan <= 0) {
      Explode();
    }
  }
  
  protected virtual void Explode()
  {
    if (GetComponent<PhotonView>().isMine) {
      PhotonNetwork.Destroy (gameObject);
    }
  }

  void OnTriggerEnter(Collider other)
  {
    IDamageable damageable = (IDamageable) other.gameObject.GetComponent(typeof(IDamageable));
  
    if (damageable != null) {
      other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damage, other.GetComponent<PhotonView>().owner, PhotonNetwork.player);
      Explode();
    }
  }
}
