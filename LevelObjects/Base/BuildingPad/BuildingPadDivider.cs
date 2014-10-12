/**
 * BuildingPadDivider
 * 
 * Because of the way the building pad is currently structured we need to split
 * the builing pad in half so when there are collisions with this object we pass
 * them onto the building pad to be placed
 */
using UnityEngine;
using System.Collections;

public class BuildingPadDivider : MonoBehaviour
{
  private BuildingPad buildingPad;

  void Awake()
  {
    buildingPad = transform.parent.GetComponent<BuildingPad> ();
    MakeTransparent();
  }

  /**
   * On trigger collison place the box on the building pad
   */
  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Box") {
      Box box = other.GetComponent<Box>();

      if (box.Enabled()) {
        buildingPad.PlaceBox(box);
      }
    }
  }

  private void MakeTransparent()
  {
    Color currentColor = renderer.material.color;
    currentColor.a = 0f;
    
    renderer.material.color = currentColor;
  }
}
