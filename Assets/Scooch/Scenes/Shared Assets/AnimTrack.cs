using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  /// <summary>
  /// Provides playback access to an AnimClip.
  /// </summary>
  public class AnimTrack : MonoBehaviour
  {
    public AnimClip animClip;

    public float Runtime
    {
      get
      {
        int n = animClip.frames.Count;
        var first = animClip.frames[0].time; // should always be zero ...
        var last = animClip.frames[n-1].time;
        return last - first;
      }
    }

    public UnityEngine.HumanPose GetPose(float time)
    {
      // Given a non-normalized time, returns a pose from the given clip...
      float clampTime = Mathf.Clamp(time, 0, Runtime); // in [0, RunTime]
      float searchTime = clampTime + animClip.frames[0].time; // ready to iterate through AnimClip

      // Search for bounding frames
      int i = 0, j = 1;
      for (; animClip.frames[j].time < searchTime; i = ++j - 1) { }

      // Retrieve bounding frames
      AnimFrame fA = animClip.frames[i];
      AnimFrame fB = animClip.frames[j];

      // Retrieve bounding poses
      HumanPose a = fA.pose;
      HumanPose b = fA.pose;

      // Interpolation factor
      float t = clampTime / Runtime;

      // Interpolate frames
      float[] muscles = new float[a.muscles.Length];
      for (int k = 0; k < a.muscles.Length; k++)
      {
        muscles[k] = Mathf.Lerp(a.muscles[k], b.muscles[k], t);
      }

      // Interpolate position
      Vector3 pA = new Vector3(a.px, a.py, a.pz);
      Vector3 pB = new Vector3(b.px, b.py, b.pz);
      Vector3 pC = Vector3.Lerp(pA, pB, t);

      // Interpolate rotation
      Quaternion rA = new Quaternion(a.rx, a.ry, a.rz, a.rw);
      Quaternion rB = new Quaternion(b.rx, b.ry, b.rz, b.rw);
      Quaternion rC = Quaternion.Lerp(rA, rB, t);

      // Apply local-to-world transform
      var p = transform.rotation * pC + transform.position;
      var r = transform.rotation * rC;

      // Return human pose
      return new UnityEngine.HumanPose()
      {
        bodyPosition = p,
        bodyRotation = r,
        muscles = muscles
      };
    }
  }

}
