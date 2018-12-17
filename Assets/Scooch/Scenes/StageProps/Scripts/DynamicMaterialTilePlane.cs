using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicMaterialTilePlane : MonoBehaviour
{
  /// <summary>
  /// The real-world size, in meters, to which the material is tiled.
  /// </summary>
  public Vector2 size = Vector2.one;

  private Renderer Renderer { get; set; }

  private void Awake()
  {
    Renderer = GetComponent<MeshRenderer>();
  }

  private void Update()
  {
    // Assume we are a UnityEngine primitive plane, i.e. at scale=<1,1,1>
    // we are 10m x 10m (xz-plane) and 0m (?) tall (y-axis).

    // Compute tiling factors
    float w = 10.0f * transform.localScale.x;
    float d = 10.0f * transform.localScale.z;
    float x = Mathf.Max(1.0f, Mathf.Round(w / size.x));
    float y = Mathf.Max(1.0f, Mathf.Round(d / size.y));

    // Apply to renderer material
    Renderer.sharedMaterial.mainTextureScale = new Vector2(x, y);
  }
}
