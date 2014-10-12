/**
 * BaseManager
 *
 * A class for managing the bases for the classic game mode
 */
using UnityEngine;
using System.Collections;

public class BaseManager : MonoBehaviour
{
  private Base[] bases;

  public static BaseManager instance { get; private set; }

  void Awake()
  {
    instance = this;
    bases = FindObjectsOfType (typeof(Base)) as Base[];
  }
  
  /**
   * Return the base with the ID equal to passed id
   */
  public Base BaseForID(int id)
  {
    Base baseForPlayer = null;
    
    foreach (Base b in bases) {
      if (b.ID == id) {
        baseForPlayer = b;
      }
    }
    
    return baseForPlayer;
  }
}
