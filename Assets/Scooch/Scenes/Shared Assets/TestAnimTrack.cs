using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  [RequireComponent(typeof(AnimClipDatabase))]
  public class TestAnimTrack : MonoBehaviour
  {
    public AnimTrackPlayback playback;

    private void Start()
    {
      // Get the first animation
      var db = GetComponent<AnimClipDatabase>();
      var clip = db.GetAnimClipAt(0);

      // Instantiate a track
      var gobj0 = new GameObject("track0");
      var track0 = gobj0.AddComponent<AnimTrack>();
      track0.animClip = clip;

      // Instantiate a track
      var gobj1 = new GameObject("track1");
      var track1 = gobj1.AddComponent<AnimTrack>();
      track1.animClip = clip;

      // Instantiate a multitrack
      var gobj2 = new GameObject("track2");
      var track2 = gobj2.AddComponent<AnimMultiTrack>();
      track2.AddTrack((AnimTrackBase)track0);
      track2.AddTrack((AnimTrackBase)track1);

      // Apply to playback
      playback.Track = (AnimTrackBase)track2;
    }
  }

}
