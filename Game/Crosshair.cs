/**
 * Crosshair
 *
 * A class for manging the crosshair for the player. Each weapon will have a different
 * crosshair which we load/cache on start.
 */
using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
  private Hashtable crosshairs = new Hashtable();
  private string path = "Crosshairs/";

  public static Crosshair instance { get; private set; }
  
  void Awake()
  {
    instance = this;
  }

  void Start()
  {
    crosshairs["Default"] = Resources.Load<Texture2D>(path + "Default");
    crosshairs["Pistol"]  = Resources.Load<Texture2D>(path + "Pistol");
    crosshairs["Shotgun"] = Resources.Load<Texture2D>(path + "Shotgun");
    crosshairs["Uzi"]     = Resources.Load<Texture2D>(path + "Uzi");
    crosshairs["GatlingGun"]  = Resources.Load<Texture2D>(path + "GatlingGun");
    crosshairs["GrenadeBelt"] = Resources.Load<Texture2D>(path + "GrenadeBelt");
    crosshairs["RocketLauncher"] = Resources.Load<Texture2D>(path + "RocketLauncher");
  }

  public void Enable()
  {
    guiTexture.enabled = true;
  }

  public void Disable()
  {
    guiTexture.enabled = false;
  }

  /**
   * Change the current Crosshair to the crosshair of the given weapon name
   */
  public void Change(string weaponName)
  {
    guiTexture.texture = (Texture2D) crosshairs[weaponName];
  }
}
