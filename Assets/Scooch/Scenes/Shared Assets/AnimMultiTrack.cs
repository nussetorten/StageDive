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
