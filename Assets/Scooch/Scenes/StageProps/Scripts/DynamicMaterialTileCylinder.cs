using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DynamicMaterialTileCylinder : MonoBehaviour
{
  public float scale = 0.05f; // 5cm

  private Renderer Renderer { get; set; }

  [ExecuteInEditMode]
  private void Awake()
  {
    Renderer = GetComponent<Renderer>(); 
  }

  [ExecuteInEditMode]
  private void Update()
  {
    // Assume we are a UnityEngine primitive cylinder, i.e. at scale=<1,1,1>
    // we are 1m wide (xz-plane) and 2m tall (y-axis).

    // Compute x-axis tiling factor
    // Approximate circumference of ellipse using Ramanujan's equation:
    float a = transform.localScale.x / 2.0f;
    float b = transform.localScale.z / 2.0f;
    float c = Mathf.PI * (3*(a+b)-Mathf.Sqrt((3*a+b)*(a+3*b)));
    float x = Mathf.Max(1.0f, Mathf.Round(c / scale));
    
    // Compute y-axis tiling as well.
    float h = transform.localScale.y * 2.0f;
    float y = Mathf.Max(1.0f, Mathf.Round(h / scale));

    // Apply to renderer material
    Renderer.material.mainTextureScale = new Vector2(x, y);
  }
}
