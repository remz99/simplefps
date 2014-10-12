/*
 * BuildingPadMesh
 * 
 * A class for the container mesh for the building pad, currently its
 * main purpose is to just change the color & transparency.
 */
using UnityEngine;
using System.Collections;

public class BuildingPadMesh : MonoBehaviour
{
  void Start()
  {
    Color currentColor = renderer.material.color;
    currentColor.a = 0.2f;
    
    renderer.material.color = currentColor;
  }
}
