/**
 *  MenuCamera
 *
 *  Camera used for the main menu system
 */
using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour
{
  public static MenuCamera instance { get; private set; }

  void Awake()
  {
    instance = this;
  }
}
