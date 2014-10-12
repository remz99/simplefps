/*
 * BaseHQ
 * 
 * A class to represent the base for each team in the Classic game mode.
 * A Base can take damage which is handled by the RPC function TakeDamage.
 */
using UnityEngine;
using System.Collections;

public class BaseHQ : MonoBehaviour, IDamageable
{
  private int  health = 100;
  private bool alive  = true;
  private Base teamBase;
  private Color color;

  void Start()
  {
    color = renderer.material.color;
    teamBase = transform.parent.GetComponent<Base>();
  }

  [RPC]
  public void TakeDamage(int damage, PhotonPlayer owner, PhotonPlayer killer)
  {
    if (alive) {
      if (health > 0) {
        health -= damage;
        StartCoroutine(Flash());
      }
    }

    if (health <= 0) {
        alive = false;
        ScoreBoard.instance.GetComponent<PhotonView>().RPC("AddKillToScoreBoard", PhotonTargets.All, killer);

        teamBase.Explode();
      }
    }
  }

  IEnumerator Flash()
  {   
    renderer.material.color = Color.white;
    yield return new WaitForSeconds(0.1f);
    renderer.material.color = color;
  }
}
