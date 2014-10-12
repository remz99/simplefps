/*
 * PlayerMovement
 * 
 * This class handles player input
 * 
 */
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
  private PlayerController playerController;
  private CharacterController characterController;
  private Camera camera;

  // movement variables
  private const float MOVEMENT_SPEED = 7.0f;

  private float verticalSpeed;
  private float horizontalSpeed;
  private float verticalVelocity = 0.0f;
  private Vector3 speedVector;
  private float dodgeCoolDown = 1.5f;
  
  // rotation variables
  private const float MOUSE_SENSITIVITY = 1.5f;
  private const float JUMP_SPEED = 3.5f;
  private const float PITCH_ROTATION_LIMIT = 60.0f;
  private const float PITCH_ROTATION_LIMIT_WITH_BOX = 30.0f;

  private float pitch = 0.0f;
  private float yaw;
  private Vector3 yawVector;

  private GameController gameController;

  // wip
  float mass = 3.0f;
  Vector3 impact = Vector3.zero;

  void Start()
  {
    playerController = GetComponent<PlayerController>();
    camera = transform.FindChild("Camera").gameObject.GetComponent<Camera>();
    characterController = GetComponent<CharacterController>();

    gameController = GameController.instance;
  }

  void Update()
  {
    if (gameController.InProgress()) {
      Move();
      Rotate();
      Dodge();

      if (impact.magnitude > 0.2F) {
        characterController.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
      }
    }
  }

  public void AddImpact(Vector3 dir, float force)
  {
    dir.Normalize();

    if (dir.y < 0) { 
      dir.y = -dir.y;
    }

  impact += dir.normalized * (force / mass);
  }

  // single tap dodge
  private void Dodge()
  {
    if (Input.GetButtonDown("Dodge")) {
      if (dodgeCoolDown < 0) {
        AddImpact(camera.transform.forward, 50.0f);
        dodgeCoolDown = 1.5f;
      }
    }

    dodgeCoolDown -= Time.deltaTime;
  }
  
  private void Move()
  {
    verticalSpeed   = Input.GetAxis ("Vertical")   * MOVEMENT_SPEED;
    horizontalSpeed = Input.GetAxis ("Horizontal") * MOVEMENT_SPEED;
    
    verticalVelocity += Physics.gravity.y * Time.deltaTime;
    
    if (characterController.isGrounded) {
      if (Input.GetButtonDown ("Jump")) {
        verticalVelocity = JUMP_SPEED;
      }
    }
    
    speedVector = new Vector3 (horizontalSpeed, verticalVelocity, verticalSpeed);
    speedVector = playerController.transform.rotation * speedVector;
    
    characterController.Move (speedVector * Time.deltaTime);
  }

  private void Rotate()
  {
    yaw    = Input.GetAxis ("Mouse X") * MOUSE_SENSITIVITY;
    pitch -= Input.GetAxis ("Mouse Y") * MOUSE_SENSITIVITY;
    
    yawVector = new Vector3 (0, yaw, 0);

    playerController.transform.Rotate(yawVector);

    // when the player has the item we want to further clamp 
    // the view to stop the box going through the ground
    if (playerController.CanPickUp()) {
      pitch = Mathf.Clamp(pitch, -PITCH_ROTATION_LIMIT, PITCH_ROTATION_LIMIT);
    } else {
      pitch = Mathf.Clamp(pitch, -PITCH_ROTATION_LIMIT_WITH_BOX, PITCH_ROTATION_LIMIT_WITH_BOX);
    }
    
    camera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
  }
}