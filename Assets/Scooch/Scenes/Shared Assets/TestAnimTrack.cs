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
      var gobj = new GameObject("track0");
      var track = gobj.AddComponent<AnimTrack>();
      track.animClip = clip;

      // Apply to playback
      playback.Track = track;
    }
  }

}
