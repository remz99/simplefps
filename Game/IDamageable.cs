/*
* IDamageable
*
* Interface for classes which can take damage from players
*/
using UnityEngine;
using System.Collections;

public interface IDamageable
{
  void TakeDamage(int damage, PhotonPlayer owner, PhotonPlayer killer);
}
