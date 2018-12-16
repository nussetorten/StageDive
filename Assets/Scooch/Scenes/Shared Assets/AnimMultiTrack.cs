using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  public class AnimMultiTrack : AnimTrackBase
  {
    public List<AnimTrackBase> m_SubTracks = new List<AnimTrackBase>();

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

    protected override void OnValidate()
    {
      // Validates PaddedClip*Time, Duration
      base.OnValidate();

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

    public override UnityEngine.HumanPose StartPose
    {
      get
      {
        return m_SubTracks[0].StartPose;
      }
    }

    public override UnityEngine.HumanPose EndPose
    {
      get
      {
        int n = m_SubTracks.Count;
        return m_SubTracks[n - 1].EndPose;
      }
    }

    public override UnityEngine.HumanPose GetPose(float time)
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
  }
}
