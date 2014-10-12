/*
 * ActionManager
 * 
 * This class handles player actions/interactions with objects in the game
 */
using UnityEngine;
using System.Collections;

public class ActionManager
{
  private PlayerController playerController;
  private CharacterController characterController;
  private Camera camera;

  private RaycastHit hit;
  private GameObject pickedUpObject;
  
  public ActionManager(PlayerController controller)
  {
    playerController  = controller;
    camera        = controller.transform.FindChild ("Camera").gameObject.GetComponent<Camera>();
    characterController = controller.GetComponent<CharacterController>();

    playerController.RegisterUpdateDelegate(Update);
  }
  
  public void Update()
  {
    Interact();
  }

  public bool CanPickUp()
  {
    return pickedUpObject == null;
  }

  private void Interact()
  {
    if (Input.GetButtonDown("PickUp")) {
      if (CanPickUp()) {
        FindPickUpObject();
      } else {
        PutDownObject();
      }
    }
  }

  private void FindPickUpObject()
  {
    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
    
    if (Physics.Raycast(ray, out hit, 2)) {     
      if (hit.transform.gameObject.tag == "Box") {
        PickUpOrBuildBox(hit.transform.GetComponent<Box>());
      }
    }
  }

  void PickUpOrBuildBox(Box box)
  {
    if (box.Enabled()) {
      PickUpBox(box);
    } else if (box.PlayerCanBuild(playerController.teamID)) {
      box.Build();
    }
  }

  void PickUpBox(Box box)
  {
    // make it so players can't fire while picking up a box
    playerController.DisableWeapons();

    pickedUpObject = box.gameObject;

    // rpc to disable the box
    box.GetComponent<PhotonView>().RPC("Disable", PhotonTargets.All, null);

    // rpc to set the boxes parent
    box.GetComponent<PhotonView>().RPC ("SetParent", PhotonTargets.All, playerController.GetComponent<PhotonView> ().viewID);
  }

  void PutDownObject()
  {
    if (pickedUpObject.gameObject.tag == "Box") {
      Box box = pickedUpObject.GetComponent<Box>();
      box.GetComponent<PhotonView>().RPC("Enable", PhotonTargets.All, null);

      // rpc to set the boxes parent to null
      box.GetComponent<PhotonView>().RPC ("SetParent", PhotonTargets.All);

      // if the player is in the air then push the box forward a bit
      if (!playerController.GetComponent<CharacterController>().isGrounded) {
        box.rigidbody.AddForce(camera.transform.forward * 4f, ForceMode.Impulse);
      }

      playerController.EnableWeapons();
    }
    
    pickedUpObject = null;
  }
}
