using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class HumanPoseRecorder : MonoBehaviour {

  public Animator target;
  public Transform root;
  [SerializeField]
  private StageDive.AnimClipDatabase m_AnimClipDatabase;

  private HumanPoseHandler m_SourcePoseHandler;
  private bool m_Recording = false;

  private void Start()
  {
    if (target != null && root != null)
    {
      m_SourcePoseHandler = new HumanPoseHandler(target.avatar, root);
    }
  }

  [ContextMenu("Start recording")]
  public void StartRecording()
  {
    m_Recording = true;
    StartCoroutine(RecordRoutine());
  }

  [ContextMenu("Stop recording")]
  public void StopRecording()
  {
    m_Recording = false;
  }

  private IEnumerator RecordRoutine()
  {
    Debug.Log("Started recording ...");
    yield return new WaitForEndOfFrame();

    float t0 = -1.0f;
    var poses = new Dictionary<float,HumanPose>();
    while (m_Recording)
    {
      float tn = Time.time;

      // Capture the start time.
      if (t0 < 0) t0 = tn;

      // Pose time stored relative to first pose.
      tn = tn - t0;
      
      HumanPose pose = new HumanPose();
      m_SourcePoseHandler.GetHumanPose(ref pose);

      poses.Add(tn, pose);

      yield return new WaitForEndOfFrame();
    }

    Debug.Log("Stopped recording, adding to database ...");

    // Build an animation clip from our time/pose pairs
    StageDive.AnimClip clip = new StageDive.AnimClip();
    foreach (var time_pose in poses)
    {
      var time = time_pose.Key;
      var pose = time_pose.Value;
      clip.frames.Add(new StageDive.AnimFrame
      {
        time = time,
        pose = new StageDive.HumanPose
        {
          px = pose.bodyPosition.x,
          py = pose.bodyPosition.y,
          pz = pose.bodyPosition.z,
          rx = pose.bodyRotation.x,
          ry = pose.bodyRotation.y,
          rz = pose.bodyRotation.z,
          rw = pose.bodyRotation.w,
          muscles = pose.muscles        
        }
      });
    }

    // Store in database
    var name = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'h'mm'm'ss's'");
    m_AnimClipDatabase.AddAnimClip(name, clip);
  }
}