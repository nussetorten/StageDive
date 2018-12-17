using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageDive;

public class TrackAutoOffset : MonoBehaviour
{
  public Transform target;

  public AnimTrackBase track;

  private void Update()
  {
    // Get start and end poses
    var p0 = track.StartPose;
    var p1 = track.EndPose;
    
    // Isolate xz-translation and y-rotation
    var t0 = new Vector3(p0.bodyPosition.x, 0, p0.bodyPosition.z);
    var r0 = Quaternion.Euler(0, p0.bodyRotation.eulerAngles.y, 0);
    var t1 = new Vector3(p1.bodyPosition.x, 0, p1.bodyPosition.z);
    var r1 = Quaternion.Euler(0, p1.bodyRotation.eulerAngles.y, 0);

    // Compute relative changes
    var td = t1 - t0;
    var rd = Quaternion.Inverse(r0) * r1;

    // Apply
    transform.position = target.position - td;
    transform.rotation = Quaternion.Inverse(rd) * target.rotation;
  }
}
