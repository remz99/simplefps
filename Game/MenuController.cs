/**
 * MenuController
 * 
 * This class handles the GUI drawing and events of the main and ingame menus
 */
using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
  private bool offline = false;
  private bool displaySubMenu = false;
  private bool displayInGameMenu = false;

  public static MenuController instance { get; private set; }

  void Awake()
  {
    instance = this;
    DontDestroyOnLoad(transform.gameObject);
  }

  /**
   * Display the ingame menu if escape is pressed or
   * toggle the scoreboard if tab is pressed
   */
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      MenuController.instance.ToggleInGameMenu();
    } else if (Input.GetKeyDown(KeyCode.Tab)) {
      if (ScoreBoard.instance) {
        ScoreBoard.instance.Toggle();
      }
    }
  }

  void OnGUI()
  {
    if (GameInProgress()) {
      RenderInGameMenu();
    } else {
      RenderMainMenu();
    }
  }

  public void ToggleInGameMenu()
  {
    displayInGameMenu = !displayInGameMenu;
  }

  private void RenderMainMenu()
  {
    GUI.BeginGroup(new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 150));

    GUI.Box(new Rect (0,0,100,150), "Menu");

    if (!displaySubMenu) {
      if (GUI.Button(new Rect(10,40,80,30), "Offline")) {
        offline = true;
        displaySubMenu = true;
      }

      if (GUI.Button(new Rect(10,70,80,30), "Online")) {
        offline = false;
        displaySubMenu = true;
      }
    } else {
      if (GUI.Button(new Rect(10,40,80,30), "Classic")) {
        NetworkManager.instance.LoadGame(offline, "Classic");
      }
      
      if (GUI.Button(new Rect(10,70,80,30), "DeathMatch")) {
        NetworkManager.instance.LoadGame(offline, "DeathMatch");
      }

      if (GUI.Button(new Rect(10,100,80,30), "Back")) {
        displaySubMenu = false;
        PhotonNetwork.Disconnect();
      }

    }   
    GUI.EndGroup ();
  }

  private void RenderInGameMenu()
  {
    if (displayInGameMenu) {
      GUI.BeginGroup (new Rect (10, 10, 100, 150));

      GUI.Box (new Rect (0, 0, 100, 150), "Menu");

      if (GUI.Button(new Rect(10, 40, 80, 30), "Exit")) {
        NetworkManager.instance.LeaveGame();
        Application.LoadLevel("Menu");
      }

      if (GUI.Button(new Rect(10, 70, 80, 30), "Controls")) {
        Debug.Log("TODO: Controls menu");
      }

      GUI.EndGroup ();
    }
  }

  private bool GameInProgress()
  {
    return GameController.instance != null;
  }
}
