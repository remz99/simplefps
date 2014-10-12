/*
 * WeaponManager
 * 
 * This class handles the weapon management for a player
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager
{
  private PlayerController playerController;
  private Camera camera;

  private RaycastHit Hit;

  private GameObject weaponPosition;
  private Weapon currentWeapon;
  private Weapon previousWeapon;

  private bool disabled = false;
  
  private Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon> (){
    {"Pistol",          null},
    {"Shotgun",         null},
    {"Uzi",             null},
    {"GatlingGun",      null},
    {"GrenadeBelt",     null},
    {"RocketLauncher",  null},
  };

  private string[] weaponKeys = new string[6]{"Pistol", "Shotgun", "Uzi", "GatlingGun", "GrenadeBelt", "RocketLauncher"};

  public WeaponManager(PlayerController controller)
  {
    playerController = controller;
    camera           = controller.transform.FindChild ("Camera").gameObject.GetComponent<Camera>();
    weaponPosition   = camera.transform.FindChild("WeaponPosition").gameObject;

    Crosshair.instance.Enable();

    playerController.RegisterUpdateDelegate(Update);
  }

  public void Update()
  {
    Fire();
    ChangeWeaponOnPlayerInput();
  }

  // Render icons for the players current weapons
  // if the player doens't have a weapon => empty square
  // else if the player has a weapon but its not active then "has"
  // if the weapon is current "active"
  // todo: add icons/textures for each state
  public void OnGUI()
  {
    int left   = 0;
    int top    = 0;
    int width  = 60;
    int height = 40;

    // if the player has the weapon then render the text
    // if enabled add a class / new stlye or something
    GUI.BeginGroup(new Rect ((Screen.width / 2) - 130, Screen.height - 45, 400, 80));
      //GUI.DrawTexture (new Rect (0, 0, 80, 80), Resources.Load<Texture2D>("Heart"));
      
      foreach (string weaponName in weapons.Keys) {
        if (currentWeapon && currentWeapon.Name() == weaponName) {
          GUI.Box(new Rect(left, top, width, height), "active");
        } else if (weapons[weaponName] != null) {
          GUI.Box(new Rect(left, top, width, height), "has");
        } else {
          GUI.Box(new Rect(left, top, width, height), "");
        }
        left += 65;
      }     

    GUI.EndGroup();

    // ammo
    if (currentWeapon) {
      GUI.BeginGroup (new Rect (0, Screen.height - 80, 80, 80));
        GUI.DrawTexture (new Rect (0, 0, 80, 80), Resources.Load<Texture2D>("Ammo"));
        GUI.Label(new Rect (29, 29, 50, 50), currentWeapon.Ammo().ToString());
      GUI.EndGroup();
    }
  }

  //
  // Add a weapon of type weaponName to the player
  // - if the player already has the weapon then just add ammo
  // - confirm it works correctly
  public void AddPickedUpWeapon(string weaponName, PhotonPlayer player)
  {
    if (weapons[weaponName]) {
      weapons[weaponName].AddAmmo(weapons[weaponName].DefaultAmmoAmount());
    } else {

      // spawn weapon for the player
      if (PhotonNetwork.player == player) {

        GameObject weaponObject = PhotonNetwork.Instantiate(weaponName, weaponPosition.transform.position, camera.transform.rotation, 0);
        Weapon weapon = weaponObject.GetComponent<Weapon>();

        // need all copies to be not pick upable
        weapon.GetComponent<PhotonView>().RPC("PickUp", PhotonTargets.All, null);

        // syncs weapon name to weapon so all players have a reference to each players weapons
        weapons[weaponName] = weapon;

        // this needs to be an rpc
        weapon.GetComponent<PhotonView>().RPC("SetParent", PhotonTargets.All, playerController.GetComponent<PhotonView>().viewID, true);

        playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, weaponName, weapon.GetComponent<PhotonView>().viewID);
      }
    }
  }


  public void ChangeWeapon(string weaponName, int viewID)
  {
    // hide all child objects
    foreach (Transform child in camera.transform) {
      if (child.name != "WeaponPosition") {
        child.gameObject.SetActive(false);
      }
    }

    if (weaponName == "null") {
      currentWeapon = null;
    } else {
      Weapon weapon = PhotonView.Find(viewID).GetComponent<Weapon>();

      if (weapon.GetComponent<PhotonView>().owner == PhotonNetwork.player) {
        previousWeapon = currentWeapon;
        currentWeapon = weapon;
        Crosshair.instance.Change(weapon.Name());
      }
      
      if (camera.transform.FindChild (weapon.Name () + "(Clone)")) {
        camera.transform.FindChild (weapon.Name () + "(Clone)").gameObject.SetActive (true);
      }
    }
  }
  
  [RPC]
  public void AddAmmo(int amount)
  {
    if (currentWeapon) {
      currentWeapon.AddAmmo(amount);
    }
  }
  
  private void Fire()
  {
    if (!disabled) {
      if (HasWeapon()) {
        if ((Input.GetButton("Fire") && currentWeapon.Automatic()) || Input.GetButtonDown ("Fire")) {
          currentWeapon.Fire();
        }
      } else {
        if (Input.GetButtonDown ("Fire2")) {
          Melee();
        }
      }
    }
  }

  
  private void Melee()
  {
    if (playerController.CanPickUp()) {
      Ray ray = camera.ScreenPointToRay(Input.mousePosition);
      
      if (Physics.Raycast(ray, out Hit, 1.0f)) {
        if (Hit.transform.gameObject.tag == "Box") {
          Hit.transform.GetComponent<Box>().Break();
        }
      }
    }
  }

  private bool HasWeapon()
  {
    return currentWeapon != null;
  }

  public void DisableWeapons()
  {
    disabled = true;

    if (currentWeapon) {
      previousWeapon = currentWeapon;
      playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, "null", 99999);
    }
  }

  public void EnableWeapons()
  {
    disabled = false;

    if (previousWeapon) {
      playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, previousWeapon.Name(), previousWeapon.GetComponent<PhotonView>().viewID);
      previousWeapon = null;
    }
  }
  
  
  private void ChangeWeaponOnPlayerInput()
  {
    if (!disabled) {
      int keyIndex = 0;

      if (Input.inputString != "" && int.TryParse(Input.inputString, out keyIndex)) {
        if (keyIndex >= 1 && keyIndex <= 6) {
          string key = weaponKeys[keyIndex - 1];

          if (weapons[key] != null) {
            playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, key, weapons[key].GetComponent<PhotonView>().viewID);
          }
        }
      } else if(Input.GetButtonDown("PreviousWeapon")) {
        ChangeToPreviousWeapon();
      } else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
        ChangeToNextAvailableWeapon(true);
      } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
        ChangeToNextAvailableWeapon(false);
      }
    }
  }

  private void ChangeToPreviousWeapon()
  {
    if (previousWeapon) {   
      playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, previousWeapon.Name(), previousWeapon.GetComponent<PhotonView>().viewID);
    }
  }
  
  private void ChangeToNextAvailableWeapon(bool positive)
  {
    int index = System.Array.IndexOf(weaponKeys, currentWeapon.Name());
    string nextWeaponName = "";

    if (positive) {
      while(true){
        int nextIndex = index + 1;

        if (weaponKeys[nextIndex] != null) {
          nextWeaponName = weaponKeys[nextIndex];
          break;
        } else {
          if (weaponKeys.Length == (nextIndex)){
            index = 0;
          } else {
            index += 1;
          }
        }
      }
    } else {
      while(true){
        int nextIndex = index - 1;
        
        if (weaponKeys[nextIndex] != null) {
          nextWeaponName = weaponKeys[nextIndex];
          break;
        } else {
          if (weaponKeys.Length == 0){
            index = 0;
          } else {
            index -= 1;
          }
        }
      }
    }

    playerController.GetComponent<PhotonView>().RPC("ChangeWeapon", PhotonTargets.All, nextWeaponName, weapons[nextWeaponName].GetComponent<PhotonView>().viewID);
  }
}
