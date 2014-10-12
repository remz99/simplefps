/**
 * Box
 * 
 * A box can be picked up by a player and placed within the building pad. A box can be merged with
 * other boxes which increases its buildLevel. Places and break boxes to spawn weapons from.
 */
using UnityEngine;
using System.Collections;

public class Box : Photon.MonoBehaviour
{
  private Color color;

  private int buildLevel = 1;
  private string[] buildOptions;

  private bool enabled = true;
  private bool buildable = false;

  private Base teamBase;

  private Vector3 networkPosition = Vector3.zero;
  private Quaternion networkRotation = Quaternion.identity;
  
  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.isWriting) {
      stream.SendNext(transform.position);
      stream.SendNext(transform.rotation);
    } else {
      if (!photonView.isMine) {
        networkPosition = Vector3.Lerp(transform.position, (Vector3)stream.ReceiveNext(), 0.1f);
        networkRotation = Quaternion.Lerp(transform.rotation, (Quaternion)stream.ReceiveNext(), 0.1f);
      }
    }   
  }

  void Awake()
  {
    color = renderer.material.color;
  
    if (GameController.instance) {
      buildOptions = GameController.instance.Weapons();
    }
  }

  public bool Enabled()
  {
    return enabled;
  }

  public bool Buildable()
  {
    return buildable;
  }

  public Base TeamBase()
  {
    return teamBase;
  }

  [RPC]
  public void TeamBase(int v)
  {
    teamBase = BaseManager.instance.BaseForID(v);
  }

  [RPC]
  public void Buildable(bool v)
  {
    buildable = v;
  }

  [RPC]
  public void Enable()
  {
    enabled = true;
    rigidbody.useGravity = true;
    rigidbody.isKinematic = false;
  }

  [RPC]
  public void Disable()
  {
    enabled = false;
    rigidbody.useGravity = false;
    rigidbody.isKinematic = true;
  }

  [RPC]
  public void DisableForBuildingPad(int v)
  {
    Disable();
    Buildable();
    TeamBase(v);
  }

  [RPC]
  public void BuildLevel(int level)
  {
    buildLevel = level;
  }

  public int BuildLevel()
  {
    return buildLevel;
  }
  
  void OnMouseEnter()
  {
    if (buildable && PlayerCanBuild(PhotonNetwork.player.ID)) {
      color.a = 0.2f;
      renderer.material.color = color;
    }
  }
  
  void OnMouseExit()
  {
    if (buildable) {
      color.a = 1f;
      renderer.material.color = color;
    }
  }

  public void Build()
  {
    // make it so only master client actually instantiates objects
    PhotonNetwork.Instantiate(buildOptions[buildLevel], NewAssetLocation(), Quaternion.identity, 0);

    GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
  }

  public void Break()
  {
    // need to update this function to send an rpc so that the master client 
    // can instantiate objects and also destroy them
    if (Random.Range(1, 10) >= 6) {
      PhotonNetwork.Instantiate ("Ammo", transform.position, Quaternion.identity, 0);
    } else {
      PhotonNetwork.Instantiate ("Health", transform.position, Quaternion.identity, 0);
    }

    GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
  }

  private Vector3 NewAssetLocation()
  {
    return transform.position += new Vector3(0f, 0.5f, 0f);
  }

  [RPC]
  public void SetParent(int viewID)
  {
    PlayerController player = PhotonView.Find (viewID).GetComponent<PlayerController>();
    transform.parent = player.transform.FindChild("Camera");
  }

  [RPC]
  public void SetParent()
  {
    transform.parent = null;
  }

  [RPC]
  private void Explode()
  {
    if (PhotonNetwork.isMasterClient) {
      PhotonNetwork.Destroy(gameObject);
      GameObject.FindGameObjectWithTag("BoxSpawnPad").GetComponent<PhotonView>().RPC("DecrementCurrentBoxCount", PhotonTargets.All, null);
    }
  }

  public bool PlayerCanBuild(int teamID)
  {
    if (buildable && (teamID == teamBase.ID)) {
      return true;
    } else {
      return false;
    }
  }
}
