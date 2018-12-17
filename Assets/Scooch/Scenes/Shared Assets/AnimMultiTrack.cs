using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  public class AnimMultiTrack : AnimTrackBase
  {
    private List<AnimTrackBase> m_SubTracks = new List<AnimTrackBase>();

    public void AddTrack(AnimTrackBase track)
    {
      m_SubTracks.Add(track);
      track.PropertiesChanged += OnSubTrackPropertiesChanged;
      track.OnValidate();
    }

    public override float UnpaddedDuration
    {
      get
      {
        float duration = 0.0f;
        foreach (var track in m_SubTracks)
        {
          duration += track.Duration;
        }
        return duration;
      }
    }
    
    [SerializeField]
    protected Matrix4x4 m_P0Inv = Matrix4x4.identity;

    //private Vector3 m_P0P = Vector3.zero;
    //private Quaternion m_P0R = Quaternion.identity;

    protected virtual void ComputeP0Inv()
    {
      // Search for HumanPose corresponding to padded start of clip
      var hp = GetPoseInSubTrackSpace(0);

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

    public override void OnValidate()
    {
      // Validates PaddedClip*Time, Duration
      base.OnValidate();

      // Re-compute position @ t0 so all generated poses are relative
      // to the start of the clip.
      if (m_SubTracks.Count > 0)
      {
        ComputeP0Inv();
      }

      // All sub-tracks should be children of this object
      for (int i = 0; i < m_SubTracks.Count; i++)
      {
        // Parent sub-track
        var track = m_SubTracks[i];
        track.transform.parent = this.transform;

        // String end-to-end starting at local zero
        if (i == 0)
        {
          track.transform.localPosition = Vector3.zero;
          track.transform.localRotation = Quaternion.identity;
        }
        else /* i > 0 */
        {
          var previousTrack = m_SubTracks[i - 1];
          var hp = previousTrack.EndPose;
          // Isolate xz-plane translation and y-axis rotation
          var t = new Vector3(hp.bodyPosition.x, 0, hp.bodyPosition.z); // translation
          var r = Quaternion.Euler(new Vector3(0, hp.bodyRotation.eulerAngles.y, 0)); // rotation
          track.transform.localPosition = t;
          track.transform.localRotation = r;
        }
      }
    }

    private void OnSubTrackPropertiesChanged(object obj, System.EventArgs args)
    {
      OnValidate();
    }

    private UnityEngine.HumanPose GetPoseInSubTrackSpace(float time)
    {
      // Compute the clapped, padded, search time.
      float clampTime = Mathf.Clamp(time, 0, Duration);
      float paddedTime = PaddedClipStartTime + clampTime;
      float searchTime = paddedTime;

      // Search through tracks to find subtrack corresponding to current time.
      AnimTrackBase track = null;
      for (int i = 0; i < m_SubTracks.Count; i++)
      {
        track = m_SubTracks[i];
        if (searchTime - track.Duration <= 0) break;
        searchTime -= track.Duration;
      }
      // searchTime should be in [0,track.Duration]

      // Return the pose sampled from the chosen subtrack.
      return (track != null) ? track.GetPose(searchTime) : new UnityEngine.HumanPose(); // safeguard against null
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
      // in sequence space.
      var hp = GetPoseInSubTrackSpace(time);

      // Apply relative-to-p0 transform
      ClipToLocal(ref hp.bodyPosition, ref hp.bodyRotation);

      // Apply local-to-world transform
      LocalToWorld(ref hp.bodyPosition, ref hp.bodyRotation);

      // Return human pose
      return hp;
    }
  }
}
