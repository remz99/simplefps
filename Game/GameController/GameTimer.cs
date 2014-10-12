/**
 * GameTimer
 * 
 * A class for managing the current time of each game, defaults to 179 seconds(3 minutes)
 * Only the master client updates the time
 */
using UnityEngine;
using System.Collections;

public class GameTimer 
{
  private float gameTime;
  private bool  inProgress = true;

  private GameController gameController;

  private const float GAME_TIME = 179;

  public GameTimer(GameController controller)
  {
    gameTime = GAME_TIME;
    gameController = controller;
    gameController.RegisterUpdateDelegate(UpdateTime);
  }

  public void GameTime(float time)
  {
    gameTime = time;
  }

  public float GameTime()
  {
    return gameTime;
  }
  
  /**
   * Update game time by Time.deltaTime on each update
   * and check if the game should be over
   */
  public void UpdateTime()
  {
    if (PhotonNetwork.isMasterClient) {
      if (inProgress) {
        gameTime -= Time.deltaTime;
      }
    }
    
    if (gameTime <= 0) {
      GameController.instance.GameOver();
    }
  }

  /**
   * Draw the game Time on the screen
   */
  public void DrawGUI()
  {
    GUI.BeginGroup(new Rect (Screen.width - 60, 20, 100, 100));
      GUI.Box (new Rect (0, 0, 50, 30), "");
      GUI.Label(new Rect(10, 5, 50, 30), DisplayTime());
    GUI.EndGroup();
  }
  
  /**
   * Return the gameTime in a readable format
   */
  private string DisplayTime()
  {
    string minutes = (Mathf.Round(gameTime / 60) - 1).ToString();
    string seconds = Mathf.Round(gameTime % 60).ToString();
    
    return (minutes + ": " + seconds);
  }
}
