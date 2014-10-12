/**
 * ScoreBoard
 * 
 * A class for manging the score of the current game. Currently the scoreboard
 * is implemented using a hash for which values are updated by an RPC(normally)
 * when a player kills another player. I am thinking of moving away from this
 * and instead use the room variables for the current PhotonRoom.
 */
using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
  private bool display = false;
  
  private Hashtable scoreBoard = new Hashtable();

  public static ScoreBoard instance { get; private set; }

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {}

  void Awake()
  {
    instance = this;
  }

  void Start()
  {
    SetScoreBoard();
  }

  void OnGUI()
  {
    if (display) {
      Render();
    }
  }

  public void Show()
  {
    display = true;
  }

  public void Hide()
  {
    display = false;
  }

  public void Toggle()
  {
    display = !display;
  }

  [RPC]
  private void AddKillToScoreBoard(PhotonPlayer killer)
  {
    scoreBoard[killer.ID] = (int)scoreBoard[killer.ID] + 1;
  }

  private void SetScoreBoard()
  {
    foreach (PhotonPlayer player in PhotonNetwork.playerList) {
      scoreBoard[player.ID] = 0;
    }
  }

  private void Render()
  {
    GUI.BeginGroup(new Rect (Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 150));
      
      GUI.Box(new Rect (0,0, 300,150), "ScoreBoard");
      
      GUI.Label(new Rect(10, 25, 250, 25), "Player");
      GUI.Label(new Rect(150, 25, 250, 25), "Kills");
      
      int height = 40;
      
      foreach (DictionaryEntry pair in scoreBoard) {
        GUI.Label(new Rect(10, height, 250, 25), pair.Key.ToString());
        GUI.Label(new Rect(150, height, 250, 25), pair.Value.ToString());
        height += 20;
      }

    GUI.EndGroup();
  }
}
