/*
 * HealthManager
 * 
 * This class handles the health for a player
 */
using UnityEngine;
using System.Collections;

public class HealthManager
{
  private PlayerController playerController;

  private const int PLAYER_HEALTH = 10; // change to 100
  private int health = PLAYER_HEALTH;

  public HealthManager(PlayerController controller) 
  {
    playerController = controller;
  }

  public void OnGUI()
  {   
    GUI.BeginGroup(new Rect (Screen.width - 80, Screen.height - 80, 80, 80));
      GUI.DrawTexture(new Rect (0, 0, 80, 80), Resources.Load<Texture2D>("Heart"));
      GUI.Label(new Rect (29, 29, 50, 50), health.ToString());
    GUI.EndGroup();
  }

  public void AddHealth(int amount)
  {
    health += amount;
  }

  public void TakeDamage(int damage, PhotonPlayer killer)
  {
    health -= damage;
    
    if (health <= 0) {
      Die(killer);
    }
  }

  private void Die(PhotonPlayer killer)
  {
    if (playerController.transform.GetComponent<PhotonView>().isMine) {
      GameController.instance.PlayerDied(killer);
      PhotonNetwork.Destroy(playerController.gameObject);
    }
  }
}
