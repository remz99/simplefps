/**
 * BuildingPad
 * 
 * Each base has a building pad, which allows "boxes" to be placed so players
 * can build items. The building pad is basically two planes as opposed to one cube
 * so that we can acturately place the boxes where they are dropped/fall to.
 * 
 * When a box is placed onto the building pad it either gets placed or merged
 * with another box depending on its location and the state of the other boxes
 * 
 * To help track what has been placed we have local variables to store each of 
 * the eight possible locations for which a box can be placed
 *
 * This code is very much a WIP and a first pass at getting this working.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingPad : MonoBehaviour
{
  // base object this BuildingPad belongs to
  private Base teamBase;

  // Boxes for the bottom row
  public Box y0x0;
  public Box y0x1;
  public Box y0z0;
  public Box y0z1;
  
  // Boxes for the top row
  public Box y1x0;
  public Box y1x1;
  public Box y1z0;
  public Box y1z1;

  // Placeholder game objects for the bottom row
  private Vector3 y0x0Position;
  private Vector3 y0x1Position;
  private Vector3 y0z0Position;
  private Vector3 y0z1Position;

  // Placeholder game objects for the top row
  private Vector3 y1x0Position;
  private Vector3 y1x1Position;
  private Vector3 y1z0Position;
  private Vector3 y1z1Position;

  // bool to keep track if the bottom four boxes have been merged
  private bool bottomRowmerged = false;

  void Start()
  {
  y0x0Position = transform.position - new Vector3(0.5f,  -0.5f, 0.5f);
  y0x1Position = transform.position - new Vector3(-0.5f, -0.5f, 0.5f);
  y0z0Position = transform.position - new Vector3(0.5f,  -0.5f, -0.5f);
  y0z1Position = transform.position - new Vector3(-0.5f, -0.5f, -0.5f);
  
  y1x0Position = transform.position - new Vector3(0.5f,  -1.5f, 0.5f);
  y1x1Position = transform.position - new Vector3(-0.5f, -1.5f, 0.5f);
  y1z0Position = transform.position - new Vector3(0.5f,  -1.5f, -0.5f);
  y1z1Position = transform.position - new Vector3(-0.5f, -1.5f, -0.5f);

    teamBase = transform.parent.GetComponent<Base>();
  }
  
  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Box") {
      Box box = other.GetComponent<Box>();

      if (box.Enabled()) {
        PlaceBox(box);
      }
    }
  }
  
  /**
   * Disable the given box, this box has been placed and can be built from
   */
  private void DisableBox(Box box, Vector3 position)
  {
    box.GetComponent<PhotonView>().RPC("Disable", PhotonTargets.All, null);

    // WIP Ideally just do this in one request
    box.GetComponent<PhotonView>().RPC ("Buildable", PhotonTargets.All, true);
    box.GetComponent<PhotonView>().RPC ("TeamBase", PhotonTargets.All, teamBase.ID);

    box.transform.position = position;
    box.transform.rotation = Quaternion.identity;
  }
  
  /** 
   * Place a box on the building pad this is done by first positioning
   * the given box and then checking for and merging boxes.
   */
  public void PlaceBox(Box box)
  {
    PositionBox(box);
    MergeBoxes();
  }
  
  /**
   * Position the given box to a vector within the cube
   */
  private void PositionBox(Box box)
  {
    Vector3 position = box.transform.position;

    if (position.y >= 1.5) {
      if (position.x <= transform.position.x) {
        if (position.z <= transform.position.z) {
          if (y0x0 && y1x0 == null) {
            y1x0 = box;
            DisableBox(box, y1x0Position);
          }
        } else {
          if (y0z0 && y1z0 == null) {
            y1z0 = box;
            DisableBox(box, y1z0Position);
          }
        }
      } else {
        if (position.z <= transform.position.z) {
          if (y0x1 && y1x1 == null) {
            y1x1 = box;
            DisableBox(box, y1x1Position);
          }
        } else {
          if (y0z1 && y1z1 == null) {
            y1z1 = box;
            DisableBox(box, y1z1Position);
          }
        }
      }

    } else {

      if (position.x <= transform.position.x) {
        if (position.z <= transform.position.z) {
          if (y0x0 == null) {
            y0x0 = box;
            DisableBox(box, y0x0Position);
          }
        } else {
          if (y0z0 == null) {
            y0z0 = box;
            DisableBox(box, y0z0Position);
          }
        }
      } else {
        if (position.z <= transform.position.z) {
          if (y0x1 == null) {
            y0x1 = box;
            DisableBox(box, y0x1Position);
          }
        } else {
          if (y0z1 == null) {
            y0z1 = box;
            DisableBox(box, y0z1Position);
          }
        }
      }
    }
  }
  
  /**
   * Merge any boxes which meet the certain criteria
   */
  private void MergeBoxes()
  {
    MergeAdjacent();

    MergeVertical();

    MergeBottomRow();

    MergeVerticalRow();

    MergeAll();
  }

  /**
   * Merge two level 1 boxes which are next to each other into a
   * level two box.
   */
  private void MergeAdjacent()
  {
    if (y0x0 && y0x0.BuildLevel() == 1) {
      if (y0x1 && y0x1.BuildLevel() == 1) {
        CreateLevel2Box(y0x0, y0x1, "sideways");
        y0x1 = y0x0;

      } else if (y0z0 && y0z0.BuildLevel() == 1) {
        CreateLevel2Box(y0x0, y0z0, "backwards");
        y0z0 = y0x0;
      }
    } else if (y0z1 && y0z1.BuildLevel() == 1) {
      if (y0z0 && y0z0.BuildLevel() == 1) {
        CreateLevel2Box(y0z0, y0z1, "sideways");
        y0z1 = y0z0;
      } else if (y0x1 && y0x1.BuildLevel() == 1) {
        CreateLevel2Box(y0x1, y0z1, "backwards");
        y0z1 = y0x1;
      }
    }

    if (y1x0 && y1x0.BuildLevel() == 1) {
      if (y1x1 && y1x1.BuildLevel() == 1) {
        CreateLevel2Box(y1x0, y1x1, "sideways");
        y1x1 = y1x0;
      } else if (y1z0 && y1z0.BuildLevel() == 1) {
        CreateLevel2Box(y1x0, y1z0, "backwards");
        y1z0 = y1x0;
      }
    } else if (y1z1 && y1z1.BuildLevel() == 1) {
      if (y1z0 && y1z0.BuildLevel() == 1){
        CreateLevel2Box(y1z0, y1z1, "sideways");
        y1z1 = y1z0;
      } else if (y1x1 && y1x1.BuildLevel() == 1) {
        CreateLevel2Box(y1x1, y1z1, "backwards");
        y1z1 = y1x1;
      }
    }
  }
 
  /**
   * Merge two level 1 boxes where one is on top of the other into a
   * level three box.
   */
  private void MergeVertical()
  {
    if ((y0x0 && y0x0.BuildLevel() == 1) && (y1x0 && y1x0.BuildLevel () == 1)) {
      Createlevel3Box(y0x0, y1x0);
      y1x0 = y0x0;
    }

    if ((y0x1 && y0x1.BuildLevel() == 1) && (y1x1 && y1x1.BuildLevel () == 1)) {
      Createlevel3Box(y0x1, y1x1);
      y1x1 = y0x1;
    }

    if ((y0z0 && y0z0.BuildLevel() == 1) && (y1z0 && y1z0.BuildLevel () == 1)) {
      Createlevel3Box(y0z0, y1z0);
      y1z0 = y0z0;
    }

    if ((y0z1 && y0z1.BuildLevel() == 1) && (y1z1 && y1z1.BuildLevel () == 1)) {
      Createlevel3Box(y0z1, y1z1);
      y1z1 = y0z1;
    }
  }

  /**
   * Merge the bottom level two boxes into one level four box
   */
  private void MergeBottomRow()
  {
    if ((y0x0 && y0x0.BuildLevel() == 2) && (y0z1 && y0z1.BuildLevel() == 2)) {
      CreateLevel4Box(y0x0, y0z1, "bottom");
      y0z0 = y0z1 = y0x1 = y0x0;
    }

    if ((y1x0 && y1x0.BuildLevel() == 2) && (y1z1 && y1z1.BuildLevel() == 2)) {
      CreateLevel4Box(y1x0, y1z1, "top");
      y1z0 = y1z1 = y1x1 = y1x0;
    }
  }

  /**
   * Merge vertical 2x2 rows into one box
   */
  private void MergeVerticalRow()
  {
    if (y0x0 && y0x0.BuildLevel() == 3) {
      if (y0x1 && y0x1.BuildLevel() == 3) {
        CreateLevel5Box(y0x0, y0x1, "sideways");
        y0x1 = y0x0;
      } else if (y0z0 && y0z0.BuildLevel() == 3) {
        CreateLevel5Box(y0x0, y0z0, "backwards");
        y0z0 = y0x0;
      }
      
    } else if (y0z1 && y0z1.BuildLevel() == 3) {
      if (y0z0 && y0z0.BuildLevel() == 3) {
        CreateLevel5Box(y0z0, y0z1, "sideways");
        y0z1 = y0z0;
      } else if (y0x1 && y0x1.BuildLevel() == 3) {
        CreateLevel5Box(y0x1, y0z1, "backwards");
        y0z1 = y0x1;
      }
    }

    if (y0x0 && y0x0.BuildLevel() == 2) {
      if (y1x0 && y1x0.BuildLevel() == 2) {
        CreateLevel5Box(y0x0, y1x0, "vertical");
        y1x0 = y0x0;
      }
    } else if (y0z1 && y0z1.BuildLevel() == 2) {
      if (y1z1 && y1z1.BuildLevel() == 2) {
        CreateLevel5Box(y0z1, y1z1, "vertical");
        y1z1 = y0z1;
      }
    }
  }
  
  /**
   * Merge into a 4x4 box
   */
  private void MergeAll()
  {
    if (y0x0 && y0x0.BuildLevel() == 4) {
      if (y1x0 && y1x0.BuildLevel() == 4) {
        CreateLevel6Box(y0x0, y1x0);
      }
    } else if (y0x0 && y0x0.BuildLevel() == 5) {
      if (y0z1 && y0z1.BuildLevel() == 5) {
        CreateLevel6Box(y0x0, y0z1);
      }
    }
  }

  /**
   * Create a level two box, ie two boxes next to each other
   */
  private void CreateLevel2Box(Box from, Box to, string direction)
  {
    // send rpc to master client to destroy the box
    to.GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);

    // set the build level to 2
    from.GetComponent<PhotonView>().RPC("BuildLevel", PhotonTargets.All, 2);

    if (direction == "sideways") {
      // ie left to right

      // increase the scale of the box
      from.transform.localScale += new Vector3(1f, 0f, 0f);

      // reposition the box
      from.transform.position += new Vector3(0.5f, 0f, 0f);
    } else {
      // front to back
      // increase the scale of the box
      from.transform.localScale += new Vector3(0f, 0f, 1f);
      
      // reposition the box
      from.transform.position += new Vector3(0.0f, 0f, 0.5f);
    }
  }

  /**
   * Create a level 3 box, ie two boxes on top
   */
  private void Createlevel3Box(Box from, Box to)
  {
    // send rpc to master client to destroy the box
    to.GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
    
    // set the build level to 3
    from.GetComponent<PhotonView>().RPC("BuildLevel", PhotonTargets.All, 3);

    // increase the scale of the box
    from.transform.localScale += new Vector3(0f, 1f, 0f);
    
    // reposition the box
    from.transform.position += new Vector3(0f, 0.5f, 0f);
  }

  /**
   * Create a level 4 box, 2x2 horizontal
   */
  private void CreateLevel4Box(Box from, Box to, string level)
  {
    // send rpc to master client to destroy the box
    to.GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
    
    // set the build leve to 4
    from.GetComponent<PhotonView>().RPC("BuildLevel", PhotonTargets.All, 4);

    // increase the scale of the box
    from.transform.localScale = new Vector3(2f, 1f, 2f);

    // reposition the box
    from.transform.position = y0x0Position + new Vector3(0.5f, 0f, 0.5f);

    if (level == "top") {
      from.transform.position += new Vector3(0f, 1f, 0f);
    }
  }

  /**
   * Create a level five box, 2x2 vertical
   */
  private void CreateLevel5Box(Box from, Box to, string direction)
  {
    // send rpc to master client to destroy the box
    to.GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
    
    // set the build level to 5
    from.GetComponent<PhotonView>().RPC("BuildLevel", PhotonTargets.All, 5);
    
    if (direction == "sideways") {
      // ie left to right
      
      // increase the scale of the box
      from.transform.localScale += new Vector3(1f, 0f, 0f);
      
      // reposition the box
      from.transform.position += new Vector3(0.5f, 0f, 0f);
    } else if (direction == "backwards") {
      // front to back

      // increase the scale of the box
      from.transform.localScale += new Vector3(0f, 0f, 1f);
      
      // reposition the box
      from.transform.position += new Vector3(0.0f, 0f, 0.5f);
    } else if (direction == "vertical") {
      // increase the scale of the box
      from.transform.localScale += new Vector3(0f, 1f, 0f);
      
      // reposition the box
      from.transform.position += new Vector3(0.0f, 0.5f, 0f);
    }
  }

  /**
   * Create a level six box, 4x4
   */
  private void CreateLevel6Box(Box from, Box to)
  {
    // send rpc to master client to destroy the box
    to.GetComponent<PhotonView>().RPC("Explode", PhotonTargets.MasterClient, null);
    
    // set the build level to 6
    from.GetComponent<PhotonView>().RPC("BuildLevel", PhotonTargets.All, 6);

    // set the scale
    from.transform.localScale = new Vector3 (2f, 2f, 2f);

    // reposition the box
    from.transform.position = y0x0Position + new Vector3 (0.5f, 0.5f, 0.5f);
  }
}
