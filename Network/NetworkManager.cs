/**
 * NetworkManager
 * 
 * This class handles the network connections with Photon
 */
using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
  
  private string guiText = null;

  private Hashtable gameModeIds = new Hashtable();
  
  private ExitGames.Client.Photon.Hashtable roomProperties;

  public static NetworkManager instance { get; private set; }

  private const int MAX_PLAYERS = 2;

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}
  
  void Awake()
  {
    instance = this;
    DontDestroyOnLoad(transform.gameObject);
    roomProperties = new ExitGames.Client.Photon.Hashtable() { { "map", 1 } };
  }
  
  void Start()
  {
    // correlates the gameMode / scene name to the scene id
    gameModeIds["Menu"]       = 0;
    gameModeIds["DeathMatch"] = 1;
    gameModeIds["Classic"]    = 2;
  }

  public void LoadGame(bool offline, string gameMode)
  {
    roomProperties["map"] = (int) gameModeIds[gameMode];

    if (offline) {
      LoadOfflineGame();
    } else {
      LoadOnlineGame();
    }
  }

  public void LeaveGame()
  {
    Disconnect();
  }

  private void LoadOfflineGame()
  {
    PhotonNetwork.offlineMode = true;
    PhotonNetwork.CreateRoom("Offline room");
  }

  private void LoadOnlineGame()
  {
    Connect();
  }
  
  private void Connect()
  {
    // don't auto join lobby because rooms are only visible when on master
    PhotonNetwork.autoJoinLobby = false;
    PhotonNetwork.ConnectUsingSettings ("1.0");
  }
  
  private void Disconnect()
  {
    //Debug.Log ("TODO: Send message to player controller that a player left");
    PhotonNetwork.autoCleanUpPlayerObjects = true;
    PhotonNetwork.LeaveRoom();
    PhotonNetwork.Disconnect();
  }

  void OnGUI()
  {
    if (guiText == null) {
      GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    } else {
      GUILayout.Label(guiText);
    }
  }

  void OnConnectedToMaster()
  {
    PhotonNetwork.JoinRandomRoom(roomProperties, MAX_PLAYERS);
  }

  void OnPhotonRandomJoinFailed()
  {
    //http://doc-api.exitgames.com/en/pun/current/pun/doc/general.html
    string[] roomPropsInLobby = {"map"};
    PhotonNetwork.CreateRoom(null, true, true, MAX_PLAYERS, roomProperties, roomPropsInLobby);
  }
  
  void OnPhotonCreateGameFailed()
  {
    Debug.Log("Creating room failed ... trying again");
    PhotonNetwork.JoinRandomRoom(roomProperties, 4);
  }

  void OnJoinedRoom()
  {
    PhotonNetwork.SetMasterClient (PhotonNetwork.player);

    SetCustomerPlayerProperties();

    if (PhotonNetwork.offlineMode) {
      LoadLevel();
    } else {
      if (PhotonNetwork.room.playerCount < MAX_PLAYERS) {
        guiText = "Waiting for players to join";
      } else {
        guiText = null;
        PhotonNetwork.room.open = false;
        PhotonNetwork.room.visible = false;
        GetComponent<PhotonView>().RPC("LoadLevel", PhotonTargets.All, null);
      }
    }
  }

  [RPC]
  public void LoadLevel()
  {
    PhotonNetwork.LoadLevel((int) roomProperties["map"]);
  }

  // called when a player joins and sets the player custom properties of :id and :color
  private void SetCustomerPlayerProperties()
  {
    ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable() { {"teamID", PhotonNetwork.player.ID } };

    PhotonNetwork.player.SetCustomProperties(table);
  }
}
