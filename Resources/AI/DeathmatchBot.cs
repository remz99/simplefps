using UnityEngine;
using System.Collections;

public class DeathmatchBot : MonoBehaviour {

  int teamID = 1;
  int health = 100;

  bool hasWeapon = false;
  Vector3 waypoint;

  private const float MOVEMENT_SPEED = 7.0f;
  
  private float verticalSpeed;
  private float horizontalSpeed;
  private float verticalVelocity = 0.0f;
  private Vector3 speedVector;
  private float dodgeCoolDown = 1.5f;

  private CharacterController characterController;

  void Start()
  {
    characterController = GetComponent<CharacterController>();

    teamID = GameObject.FindGameObjectsWithTag("Bot").Length;
    SetTeamColor(); 
  }

  void Awake()
  {
    SetTeamColor();
  }

  void Update()
  {
    if (waypoint == Vector3.zero) {
      if (hasWeapon) {
        Fire();
      } else {
        FindNextWaypoint();
      }
    } else {
      Move();
    }
  }

  private void FindNextWaypoint()
  {
    if (hasWeapon) {
      // shoot at closest enemy
    } else {
      if (health <= 25) {
      MoveToClosestHealth();
    } else {
      MoveToClosestWeapon();
    }
    }
  }

  private void Move()
  {
    // TODO
  }

  private void Fire()
  {
    // TODO
  }

  private void MoveToClosestHealth()
  {
    // TODO
  }

  /**
   * set movingToPosition to vector of closest weapon spawn
   */
  private void MoveToClosestWeapon()
  {
    GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
    float distance = 0f;
    Vector3 newWaypoint = Vector3.zero;

    if (weapons.Length > 0) {
      foreach(GameObject weapon in weapons) {
        if (Vector3.Distance(transform.position, weapon.transform.position) > distance) {
        newWaypoint = weapon.transform.position;
      }
    }
    } else {
      Debug.Log("no weapons to pickup");
    }

    waypoint = newWaypoint;
  }

  private void SetTeamColor()
  {
    Color teamColor = Color.blue;

    if (teamID == 1){
      teamColor = Color.blue;
    } else if (teamID == 2) {
      teamColor = Color.yellow;
    } else if (teamID == 3) {
      teamColor = Color.green;
    }

    transform.FindChild("Mesh").renderer.material.color = teamColor;
  }
}
