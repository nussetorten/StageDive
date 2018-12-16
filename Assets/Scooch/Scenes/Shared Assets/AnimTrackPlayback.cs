using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  public class AnimTrackPlayback : MonoBehaviour
  {

    public bool useScrub = false;

    [Range(0, 1)]
    public float scrub = 0f;

    public Avatar Avatar { get { return m_Avatar; } }

    public Transform Target { get { return m_Target; } }
    
    public AnimTrackBase Track
    {
      get { return m_AnimTrack; }
      set { m_AnimTrack = value; }
    }

    [SerializeField]
    private Avatar m_Avatar;
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private AnimTrackBase m_AnimTrack;
    private HumanPoseHandler m_PoseHandler;

    public void SetAvatarAndTarget(Avatar avatar, Transform target)
    {
      m_Avatar = avatar;
      m_Target = target;
      ValidatePoseHandler();
    }

    private void ValidatePoseHandler()
    {
      if (m_Avatar != null && m_Target != null)
      {
        m_PoseHandler = new HumanPoseHandler(m_Avatar, m_Target);
      }
      else
      {
        m_PoseHandler = null;
      }
    }

    private void OnValidate()
    {
      ValidatePoseHandler();
    }

    private void Start()
    {
    }

    private void Update()
    {
      // Sanity-check.
      if (m_AnimTrack == null) return;
      if (m_PoseHandler == null) return;

      // Compute the animation time.
      float runtime = m_AnimTrack.Duration;
      int n = (int)(Time.time / runtime);
      float time;
      if (useScrub)
      {
        // Time determined by scrubber.
        time = runtime * scrub;
      }
      else
      {
        // Time determined by game time, loops over [0,runtime]
        time = Time.time - (n * runtime);
        scrub = time / runtime;
      }

      // Fetch pose.
      var pose = m_AnimTrack.GetPose(time);

      // Apply to avatar.
      m_PoseHandler.SetHumanPose(ref pose);
    }

  }

}