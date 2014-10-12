/*
 * PlayerController
 * 
 */
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamageable
{ 
  private WeaponManager  playerWeaponManager;
  private ActionManager  playerActionManager;
  private HealthManager  playerHealthManager;
  private GameController gameController;

  public int teamID;
  private Base teamBase;
  private Color color;
  private bool enableGUI = true;

  public delegate void UpdateDelegate();
  public UpdateDelegate onUpdateDelegate;

  public void RegisterUpdateDelegate(UpdateDelegate del)
  {
    onUpdateDelegate += del;
  }
  
  void Start()
  {
    playerActionManager = new ActionManager(this);
    playerWeaponManager = new WeaponManager(this);
    playerHealthManager = new HealthManager(this);

    gameController = GameController.instance;

    Screen.lockCursor = true;

    // if classic mode then each player belongs to a base

    // id of the player which owns this object
    teamID = GetComponent<PhotonView>().owner.ID;

    SetTeamColor();

    if (BaseManager.instance) {
      teamBase = BaseManager.instance.BaseForID(teamID);
    }
  }
  
  public void Update()
  {
    if (gameController.InProgress()) {
      onUpdateDelegate();
    } else {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        MenuController.instance.ToggleInGameMenu();
      }
    }
  }

  void OnGUI()
  {
    if (enableGUI) {
      playerHealthManager.OnGUI();
      playerWeaponManager.OnGUI();
    }
  }

  [RPC]
  public void AddHealth(int amount, PhotonPlayer player)
  {
    if (GetComponent<PhotonView>().owner == player) {
      playerHealthManager.AddHealth (amount);
    }
  }

  [RPC]
  public void TakeDamage(int damage, PhotonPlayer player, PhotonPlayer killer)
  {
    if (GetComponent<PhotonView>().owner == player) {
    StartCoroutine(Flash());
      playerHealthManager.TakeDamage(damage, killer);
    }
  }

  [RPC]
  public void AddAmmo(int amount, PhotonPlayer player)
  {
    if (GetComponent<PhotonView>().owner == player) {
      playerWeaponManager.AddAmmo(amount);
    }
  }
  
  [RPC]
  public void AddWeapon(string weaponName, PhotonPlayer player)
  {
    playerWeaponManager.AddPickedUpWeapon(weaponName, player);
  }

  [RPC]
  public void ChangeWeapon(string weaponName, int viewID)
  {
    playerWeaponManager.ChangeWeapon(weaponName, viewID);
  }

  [RPC] 
  public void DestroyObject(int viewID)
  {
    if (PhotonNetwork.isMasterClient) {
      PhotonNetwork.Destroy(PhotonView.Find(viewID));
    }
  }

  [RPC] 
  public void DestroyObjectByOwner(PhotonPlayer owner, int viewID)
  {
    if (GetComponent<PhotonView>().owner == owner) {
      PhotonNetwork.Destroy (PhotonView.Find(viewID));
    }
  }


  [RPC]
  public void CreateObject(string prefabName, Vector3 position, Quaternion rotation, int group)
  {
    if (PhotonNetwork.isMasterClient) {
      PhotonNetwork.Instantiate(prefabName, position, rotation, group);
    }
  }

  public bool CanPickUp()
  {
    return playerActionManager.CanPickUp();
  }

  // change the color of the player mesh based on the players id
  private void SetTeamColor()
  {
    Color teamColor = Color.red;

    if (teamID == 1) {
      teamColor = Color.red;
    } else if (teamID == 2) {
      teamColor = Color.blue;
    } else if (teamID == 3) {
      teamColor = Color.yellow;
    } else if (teamID == 4) {
      teamColor = Color.green;
    }

    transform.FindChild("Mesh").renderer.material.color = teamColor;
  }

  public void DisableWeapons()
  {
    playerWeaponManager.DisableWeapons();
  }

  public void EnableWeapons()
  {
    playerWeaponManager.EnableWeapons();
  }

  [RPC]
  public void EndGame(PhotonPlayer player)
  {
    if (PhotonNetwork.player == player) { 
      enableGUI = false;

      Crosshair.instance.Disable();

      MenuCamera.instance.gameObject.SetActive(true);

      PhotonNetwork.Destroy(gameObject);

      ScoreBoard.instance.Toggle();
    }
  }

  public void Enable()
  {
    // enable player movement
    GetComponent<PlayerMovement>().enabled = true;

    // Disable the menu camera
    MenuCamera.instance.gameObject.SetActive(false);

    // Enable the camera for this player
    transform.FindChild ("Camera").gameObject.GetComponent<Camera>().enabled = true;
  }

  /**
   * Flash the mesh white on collison
   */
   IEnumerator Flash()
   {    
      renderer.material.color = Color.white;
      yield return new WaitForSeconds(0.1f);
      renderer.material.color = color;
   }
}
