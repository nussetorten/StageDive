using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fence captures an object and keeps it bounded within the fence.
/// </summary>
public class Fence : MonoBehaviour
{
  [Range(1.0f, 10.0f)]
  public float size = 5.0f;

  public List<Transform> targets;

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(new Vector3(0, size, 0), Vector3.one * size * 2);
  }
  private void Update()
  {
    foreach (var t in targets)
    {
      // Warp to other side of fence
      Vector3 p = t.position;
      if (p.x > size) p.x = -size;
      if (p.x < -size) p.x = size;
      if (p.y > 2*size) p.y = 0;
      if (p.y < 0) p.y = 2*size;
      if (p.z > size) p.z = -size;
      if (p.z < -size) p.z = size;
      t.position = p;
    }
  }
}
