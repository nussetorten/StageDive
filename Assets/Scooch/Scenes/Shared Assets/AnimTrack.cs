﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  /// <summary>
  /// Provides playback access to an AnimClip.
  /// </summary>
  public class AnimTrack : AnimTrackBase
  {
    public AnimClip animClip;

    public override float UnpaddedDuration
    {
      get
      {
        int n = animClip.frames.Count;
        var first = animClip.frames[0].time; // should always be zero ...
        var last = animClip.frames[n-1].time;
        return last - first;
      }
    }

    [SerializeField]
    protected Matrix4x4 m_P0Inv = Matrix4x4.identity;

    public override void OnValidate()
    {
      // Validates PaddedClip*Time, Duration
      base.OnValidate();
      
      // Re-compute position @ t0 so all generated poses are relative
      // to the start of the clip.
      if (animClip != null)
      {
        ComputeP0Inv();
      }
    }

    //private Vector3 m_P0P = Vector3.zero;
    //private Quaternion m_P0R = Quaternion.identity;

    protected virtual void ComputeP0Inv()
    {
      // Search for HumanPose corresponding to padded start of clip
      var hp = GetPoseInClipSpace(0);

      // Isolate xz-plane translation and y-axis rotation
      var t = new Vector3(hp.bodyPosition.x, 0, hp.bodyPosition.z); // translation
      var r = Quaternion.Euler(new Vector3(0, hp.bodyRotation.eulerAngles.y, 0)); // rotation
      var s = Vector3.one; // scale
      // Compute inverse transformation
      var P0 = Matrix4x4.TRS(t, r, s);
      m_P0Inv = P0.inverse;

      //m_P0P = new Vector3(hp.bodyPosition.x, 0, hp.bodyPosition.z);
      //m_P0R = Quaternion.Euler(new Vector3(0, hp.bodyRotation.eulerAngles.y, 0));
    }

    private UnityEngine.HumanPose GetPoseInClipSpace(float time)
    {
      // Given a non-normalized time, returns a pose from the given clip...
      float clampTime = Mathf.Clamp(time, 0, Duration); // in [0, Duration]
      float paddedTime = PaddedClipStartTime + clampTime; // seconds from start of clip
      float searchTime = paddedTime + animClip.frames[0].time; // ready to iterate through AnimClip
      // NOTE: animClip.frames[0].time is usually zero i.e. paddedTime == searchTime

      // Search for bounding frames
      // NOTE: binary search would be better ...
      int i = 0, j = 1;
      for (; animClip.frames[j].time < searchTime; i = ++j - 1) { }

      // Retrieve bounding frames
      AnimFrame fA = animClip.frames[i];
      AnimFrame fB = animClip.frames[j];

      // Retrieve bounding poses
      HumanPose a = fA.pose;
      HumanPose b = fA.pose;

      // Interpolation factor
      float diff = fB.time - fA.time;
      float t = (diff > 0) ? (searchTime - fA.time) / diff : 0.0f; // guard against DIV/0
      
      // Interpolate muscles
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

      // Return human pose
      return new UnityEngine.HumanPose()
      {
        bodyPosition = pC,
        bodyRotation = rC,
        muscles = muscles
      };
    }

    private void ClipToLocal(ref Vector3 position, ref Quaternion rotation)
    {
      //position = position - m_P0P;
      //rotation = Quaternion.Inverse(m_P0R) * rotation;
      position = m_P0Inv.MultiplyPoint3x4(position);
      rotation = m_P0Inv.rotation * rotation;
    }

    private void LocalToWorld(ref Vector3 position, ref Quaternion rotation)
    {
      position = transform.localRotation * position + transform.localPosition;
      rotation = transform.localRotation * rotation;
    }

    public override UnityEngine.HumanPose GetPose(float time)
    {
      // Retrieve the corresponding HumanPose, with position and rotation
      // in AnimClip space.
      var hp = GetPoseInClipSpace(time);

      // Apply relative-to-p0 transform
      ClipToLocal(ref hp.bodyPosition, ref hp.bodyRotation);

      // Apply local-to-world transform
      LocalToWorld(ref hp.bodyPosition, ref hp.bodyRotation);

      // Return human pose
      return hp;
    }
  }

}
