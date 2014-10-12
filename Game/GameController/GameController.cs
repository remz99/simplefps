/**
 * GameController
 * 
 * A class to handle the current game and its state. Once it has been loaded
 * a game will start with the singleton instance existing between scenes.
 */
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
  private bool inProgress = true;
  private GameMode gameMode;
  private GameTimer gameTimer;
  private PlayerManager playerManager;

  public static GameController instance { get; private set; }

  /**
   * A Delegate which gets called on every update
   */ 
  public delegate void UpdateDelegate();
  public UpdateDelegate onUpdateDelegate;

  /**
   * Add a function to the delegate
   */
  public void RegisterUpdateDelegate(UpdateDelegate del)
  {
    onUpdateDelegate += del;
  }

  void Awake()
  {
    instance = this;
    gameMode = GameMode.GameModeFor(Application.loadedLevel);
    playerManager = new PlayerManager(this);
    gameTimer = new GameTimer(this);
  }
  
  void Start()
  { 
    if (PhotonNetwork.offlineMode) {
      //PhotonNetwork.Instantiate("AI/DeathmatchBot", PlayerSpawnManager.instance.newSpawnPosition(), Quaternion.identity, 0);
      gameMode.LoadBots();
    }
  }

  void Update()
  {
    onUpdateDelegate();
  }
  
  /**
   * For Photon updates send the current progress state
   * and the current game time
   */
  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.isWriting) {
      stream.SendNext(inProgress);
      stream.SendNext(gameTimer.GameTime());
    } else {
      inProgress = (bool) stream.ReceiveNext();
      gameTimer.GameTime((float) stream.ReceiveNext());
    }   
  }
  
  /**
   * Draw the current game time on the screen and render
   * the scoreboard if the game is no longer in progress
   */
  void OnGUI()
  {
    gameTimer.DrawGUI();

    if (!inProgress) {
      ScoreBoard.instance.Show();
    }
  }
  
  /**
   * When a player has died we want to pass on the info
   * to the game mode which will handle how the score is
   * handled and if the player should be respawned.
   */
  public void PlayerDied(PhotonPlayer killer)
  {
    gameMode.PlayerDied(playerManager, killer);
  }
  
  public void GameOver()
  {
    inProgress = false;
  }

  public bool InProgress()
  {
    return inProgress;
  }

  public string[] Weapons()
  {
    return gameMode.Weapons();
  }
}
