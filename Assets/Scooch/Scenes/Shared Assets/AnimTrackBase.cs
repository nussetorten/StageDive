using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageDive
{
  public abstract class AnimTrackBase : MonoBehaviour
  {
    public event System.EventHandler PropertiesChanged;

    public abstract float UnpaddedDuration { get; }

    public float PaddedClipStartTime
    {
      get { return m_LeftPadding * UnpaddedDuration; }
    }

    public float PaddedClipEndTime
    {
      get { return (1.0f - m_RightPadding) * UnpaddedDuration; }
    }

    public float Duration
    {
      get { return (1.0f - m_LeftPadding - m_RightPadding) * UnpaddedDuration; }
    }

    public UnityEngine.HumanPose StartPose
    {
      get { return GetPose(0.0f); }
    }

    public UnityEngine.HumanPose EndPose
    {
      get { return GetPose(Duration); }
    }

    public virtual void OnValidate()
    {
      // Ensure padding doesn't exceed 100% of our animation clip.
      m_LeftPadding = Mathf.Clamp01(m_LeftPadding);
      m_RightPadding = Mathf.Clamp01(m_RightPadding);
      if (m_LeftPadding + m_RightPadding > 1.0f)
        m_RightPadding = 1.0f - m_LeftPadding;

      // Notify downstream
      if (PropertiesChanged != null)
      {
        PropertiesChanged(this, null);
      }
    }

    protected virtual void Start()
    {
      OnValidate();
    }

    [SerializeField]
    [Range(0, 1)]
    protected float m_LeftPadding = 0.0f; // normalized

    [SerializeField]
    [Range(0, 1)]
    protected float m_RightPadding = 0.0f; // normalized 

    public abstract UnityEngine.HumanPose GetPose(float time);
  }

}