/**
 * NetworkObject
 * 
 * This class is used to send the position and rotation of the attached transform
 * over PhotonUnity to the other clients. We then update the attributes with a Lerp
 * to provide smoothing.
 */
using UnityEngine;
using System.Collections;

public class NetworkObject : Photon.MonoBehaviour
{
  protected Vector3 networkPosition = Vector3.zero;
  protected Quaternion networkRotation = Quaternion.identity;

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.isWriting) {
      stream.SendNext(transform.position);
      stream.SendNext(transform.rotation);
    } else {
      networkPosition = (Vector3)stream.ReceiveNext();
      networkRotation = (Quaternion)stream.ReceiveNext();
    }   
  }

  public virtual void Update()
  {
    UpdateNetworkProperties();
  }
  
  public virtual void UpdateNetworkProperties()
  {
    if (!photonView.isMine) {
      transform.position = Vector3.Lerp(transform.position, networkPosition, 0.1f);
      transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 0.1f);
    }
  }
}
